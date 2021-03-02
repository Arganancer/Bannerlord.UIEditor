using System;
using System.Windows.Media;
using System.Xml;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public class SceneManager : Module, ISceneManager
    {
        public DrawableWidgetViewModel? RootWidget
        {
            get => m_RootWidget;
            set
            {
                if (m_RootWidget != value)
                {
                    if (m_RootWidget is not null)
                    {
                        m_CanvasEditorControl.Canvas.Dispatcher.Invoke(() =>
                        {
                            CompositionTarget.Rendering -= OnTick;
                        });
                    }
                    m_RootWidget = value;
                    OnRootWidgetChanged(m_RootWidget);
                    if (m_RootWidget is not null)
                    {
                        m_CanvasEditorControl.Canvas.Dispatcher.Invoke(() =>
                        {
                            CompositionTarget.Rendering += OnTick;
                        });
                    }
                }
            }
        }

        private DrawableWidgetViewModel? m_RootWidget;
        private IGlobalEventManager? m_GlobalEventManager;
        private InvokeGlobalEvent? m_OnRefreshLiveEditingScreen;
        private ICanvasEditorControl m_CanvasEditorControl = null!;
        public event EventHandler<DrawableWidgetViewModel?>? RootWidgetChanged;

        public XmlDocument ToXml()
        {
            XmlDocument document = new();
            var prefabNode = document.AppendChild(document.CreateElement("Prefab"));
            var windowNode = prefabNode.AppendChild(document.CreateElement("Window"));
            RootWidget?.ToXml(windowNode);
            if (!windowNode.HasChildNodes)
            {
                windowNode.AppendChild(document.CreateElement("Widget"));
            }

            return document;
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            RegisterModule<ISceneManager>();
        }

        public override void Load()
        {
            base.Load();

            m_CanvasEditorControl = PublicContainer.GetModule<ICanvasEditorControl>();
            m_GlobalEventManager = PublicContainer.GetModule<IGlobalEventManager>();
            m_OnRefreshLiveEditingScreen = m_GlobalEventManager.GetEventInvoker("OnRefreshLiveEditingScreen", this, true);
        }

        public override void Unload()
        {
            RootWidget = null;
            base.Unload();
        }

        protected virtual void OnRootWidgetChanged(DrawableWidgetViewModel? _e)
        {
            RootWidgetChanged?.Invoke(this, _e);
        }

        private void OnTick(object _sender, EventArgs _e)
        {
            if (RootWidget?.Update() ?? false)
            {
                m_OnRefreshLiveEditingScreen!(ToXml());
            }
        }
    }
}
