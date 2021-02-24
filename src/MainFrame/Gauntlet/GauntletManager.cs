using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.Core.UIExtender;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;

namespace Bannerlord.UIEditor.MainFrame.Gauntlet
{
    public class GauntletManager : Module, IGauntletManager
    {
        public UIEditorGauntletScreen UIEditorGauntletScreen { get; private set; } = null!;

        public UIContext? UIContext => UIEditorGauntletScreen.UIEditorGauntletLayer?._gauntletUIContext;
        private IGlobalEventManager? m_GlobalEventManager;
        private IUIExtenderManager m_UIExtenderManager;

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            InitializeUIEditorGauntletScreen();

            RegisterModule<IGauntletManager>();
        }

        public override void Load()
        {
            base.Load();

            m_UIExtenderManager = PublicContainer.GetModule<IUIExtenderManager>();
            m_UIExtenderManager.Enable("Bannerlord.UIEditor.Core", typeof( LiveEditingScreenInsertPatch ).Assembly);

            m_GlobalEventManager = PublicContainer.GetModule<IGlobalEventManager>();
            m_GlobalEventManager.GetEvent("OnRefreshLiveEditingScreen").EventHandler += OnRefreshLiveEditingScreen;

            UIEditorGauntletScreen.Load();
            ScreenManager.PushScreen(UIEditorGauntletScreen);
        }

        public override void Unload()
        {
            if (ScreenManager.TopScreen.Equals(UIEditorGauntletScreen))
            {
                ScreenManager.PopScreen();
            }

            UIEditorGauntletScreen.Unload();

            m_UIExtenderManager.Disable("Bannerlord.UIEditor.Core");

            base.Unload();
        }

        protected override void Dispose(bool _disposing)
        {
            UIEditorGauntletScreen.Dispose();

            base.Dispose(_disposing);
        }

        private void OnRefreshLiveEditingScreen(object _sender, IEnumerable<object> _params)
        {
            LiveEditingScreenInsertPatch.LiveEditingScreenXmlDocument = (_params.FirstOrDefault() as XmlDocument)!;
            UIEditorGauntletScreen.RefreshLiveEditingScreen();
        }

        private void InitializeUIEditorGauntletScreen()
        {
            UIEditorGauntletScreen = (UIEditorGauntletScreen)ViewCreatorManager.CreateScreenView<UIEditorGauntletScreen>();
            UIEditorGauntletScreen.Create(PublicContainer);
        }
    }
}
