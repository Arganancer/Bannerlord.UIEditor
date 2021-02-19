﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bannerlord.UIEditor.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.PrefabSystem;
using Module = Bannerlord.UIEditor.Core.Module;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class WidgetManager : Module, IWidgetManager
    {
        public IReadOnlyList<IWidgetTemplate> WidgetTemplates => m_WidgetTemplates.SelectMany(_x => _x.Value).ToList();

        internal WidgetFactory WidgetFactory => UIResourceManager.WidgetFactory;

        private Dictionary<Assembly, List<WidgetTemplate>> m_WidgetTemplates = null!;

        public UIEditorWidget CreateWidget(UIContext _context, IWidgetTemplate _widgetTemplate)
        {
            return ((WidgetTemplate)_widgetTemplate).CreateInstance(WidgetFactory, _context);
        }

        public void LoadAssembly(Assembly _assembly)
        {
            if (!m_WidgetTemplates.ContainsKey(_assembly))
            {
                List<WidgetTemplate> widgetTemplates = WidgetScraper.ScrapeAssembly(_assembly).ToList();
                m_WidgetTemplates.Add(_assembly, widgetTemplates);
            }
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            m_WidgetTemplates = new Dictionary<Assembly, List<WidgetTemplate>>();
            LoadAssembly(typeof( Widget ).Assembly);
            RegisterModule<IWidgetManager>();
        }
    }
}
