using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bannerlord.UIEditor.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.GauntletUI.PrefabSystem;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Party;
using Module = Bannerlord.UIEditor.Core.Module;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public interface IWidgetCategory
    {
        string Name { get; }
        IReadOnlyList<IWidgetTemplate> WidgetTemplates { get; }
    }

    public class WidgetManager : Module, IWidgetManager
    {
        public bool IsWorking
        {
            get => m_IsWorking;
            set
            {
                if (m_IsWorking != value)
                {
                    m_IsWorking = value;
                    OnIsWorkingChanged(m_IsWorking);
                }
            }
        }

        public IReadOnlyList<IWidgetCategory> WidgetTemplateCategories => m_WidgetTemplates.Select(_x => _x.Value).ToList();

        internal WidgetFactory WidgetFactory => UIResourceManager.WidgetFactory;
        private CancellationTokenSource CancellationTokenSource => m_LazyCancellationToken.Value;

        private Dictionary<Assembly, WidgetCategory> m_WidgetTemplates = null!;
        private bool m_IsWorking;

        private readonly BlockingCollection<Assembly> m_QueuedAssemblies = new(new ConcurrentQueue<Assembly>());
        private readonly object m_Lock = new();
        private Thread? m_AssemblyLoaderThread;

        private readonly Lazy<CancellationTokenSource> m_LazyCancellationToken = new(() => new CancellationTokenSource());
        public event EventHandler<bool>? IsWorkingChanged;

        public UIEditorWidget CreateWidget(UIContext _context, IWidgetTemplate _widgetTemplate)
        {
            return ((WidgetTemplate)_widgetTemplate).CreateInstance(WidgetFactory, _context);
        }

        public void LoadAssembly(Assembly _assembly)
        {
            if (!m_WidgetTemplates.ContainsKey(_assembly))
            {
                m_QueuedAssemblies.Add(_assembly);
                lock (m_Lock)
                {
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    if (m_AssemblyLoaderThread is null)
                    {
                        m_AssemblyLoaderThread = new Thread(ProcessQueuedAssemblies);
                        m_AssemblyLoaderThread.Start(CancellationTokenSource.Token);
                    }
                }
            }
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            m_WidgetTemplates = new Dictionary<Assembly, WidgetCategory>();

            LoadAssembly(typeof( Widget ).Assembly);
            LoadAssembly(typeof( FillBar ).Assembly);
            LoadAssembly(typeof( HintWidget ).Assembly);


            RegisterModule<IWidgetManager>();
        }

        protected override void Dispose(bool _disposing)
        {
            lock (m_Lock)
            {
                CancellationTokenSource.Cancel();
            }

            m_AssemblyLoaderThread?.Join();

            base.Dispose(_disposing);
        }

        protected virtual void OnIsWorkingChanged(bool _e)
        {
            IsWorkingChanged?.Invoke(this, _e);
        }

        private void ProcessQueuedAssemblies(object _cancellationToken)
        {
            var cancellationToken = (CancellationToken)_cancellationToken;
            IsWorking = true;
            var nextAssemblyExists = m_QueuedAssemblies.TryTake(out Assembly assembly);
            while (nextAssemblyExists)
            {
                List<WidgetTemplate> widgetTemplates = WidgetScraper.ScrapeAssembly(assembly, cancellationToken).ToList();
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (widgetTemplates.Any())
                {
                    m_WidgetTemplates.Add(assembly, new WidgetCategory(widgetTemplates, assembly));
                }

                lock (m_Lock)
                {
                    nextAssemblyExists = m_QueuedAssemblies.TryTake(out assembly);
                    if (!nextAssemblyExists)
                    {
                        IsWorking = false;
                        return;
                    }
                }
            }
        }

        private class WidgetCategory : IWidgetCategory
        {
            public string Name => ParentAssembly.GetName().Name;
            public IReadOnlyList<IWidgetTemplate> WidgetTemplates => MutableWidgetTemplates;
            public List<WidgetTemplate> MutableWidgetTemplates { get; }
            public Assembly ParentAssembly { get; }

            public WidgetCategory(List<WidgetTemplate> _mutableWidgetTemplates, Assembly _parentAssembly)
            {
                MutableWidgetTemplates = _mutableWidgetTemplates;
                ParentAssembly = _parentAssembly;
            }
        }
    }
}
