using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public class LayoutManager : Module, ILayoutManager
    {
        public static LayoutManager Instance { get; private set; } = null!;
        private Dictionary<Dock, ParentContainer> m_LayoutContainers = null!;

        private readonly List<ConnectedUserControl> m_Panels = new();

        private IMainWindow m_MainWindow = null!;
        private ISettingsManager m_SettingsManager = null!;

        public void Refresh()
        {
            foreach (var layoutContainer in m_LayoutContainers)
            {
                layoutContainer.Value.Refresh();
            }
        }

        public T? FindContainer<T>(Control _control) where T : LayoutContainer
        {
            foreach (KeyValuePair<Dock, ParentContainer> container in m_LayoutContainers)
            {
                T? output = container.Value.FindContainer<T>(_control);
                if(output is not null)
                {
                    return output;
                }
            }

            return null;
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            Instance = this;

            RegisterModule<ILayoutManager>();

            PublicContainer.ConnectToModule<IMainWindow>(this, OnMainWindowRegistered, _ => { });
        }

        public override void Load()
        {
            base.Load();

            m_SettingsManager = PublicContainer.GetModule<ISettingsManager>();

            m_MainWindow.Window.Dispatcher.Invoke(() =>
            {
                foreach (ConnectedUserControl panel in m_Panels)
                {
                    panel.Load();
                }

                ApplyLayout();
            });
        }

        public override void Unload()
        {
            m_MainWindow.Window.Dispatcher.Invoke(() =>
            {
                foreach (ConnectedUserControl panel in m_Panels)
                {
                    panel.Unload();
                }
            });

            base.Unload();
        }

        protected override void Dispose(bool _disposing)
        {
            m_MainWindow.Window.Dispatcher.Invoke(() =>
            {
                foreach (ConnectedUserControl panel in m_Panels)
                {
                    panel.Dispose();
                }
            });

            base.Dispose(_disposing);
        }

        public void AddLayoutElement(ConnectedUserControl _panel)
        {
            IPanel panelInterface = (IPanel)_panel!;
            string panelName = panelInterface.PanelName;
            var panelAttribute = _panel.GetType().GetCustomAttribute<PanelAttribute>();
            if (panelAttribute is null)
            {
                return;
            }

            ISettingCategory panelSettings = m_SettingsManager.GetSettingCategory($"{panelName}Settings");
            panelInterface.SettingCategory = panelSettings;
            if (!panelSettings.GetSetting(nameof( panelAttribute.IsOpen ), panelAttribute.IsOpen))
            {
                return;
            }


            string path = panelSettings.GetSetting("Path", panelAttribute.Path)!;
            var desiredWidth = panelSettings.GetSetting("Width", panelAttribute.Width)!;
            var desiredHeight = panelSettings.GetSetting("Height", panelAttribute.Height)!;
            var isSelected = panelSettings.GetSetting("IsSelected", panelAttribute.IsSelected)!;

            string[] splitPath = path.Split(';');
            var initialDock = (Dock)Enum.Parse(typeof( Dock ), splitPath[0]);
            ParentContainer currentParent = m_LayoutContainers[initialDock];
            if (splitPath.Length > 1)
            {
                currentParent.InsertPanel(splitPath.SubArray(1), desiredWidth, desiredHeight, isSelected, panelInterface);
            }
            else
            {
                currentParent.InsertPanel(new[] {"HorizontalLayoutContainer:Left"}, desiredWidth, desiredHeight, isSelected, panelInterface);
            }
        }

        private void OnMainWindowRegistered(IMainWindow _mainWindow)
        {
            m_MainWindow = _mainWindow;
            List<Type> panelTypes = typeof( LayoutManager ).Assembly.SafeGetTypes(_type =>
                typeof( ConnectedUserControl ).IsAssignableFrom(_type) &&
                typeof( IPanel ).IsAssignableFrom(_type)).ToList();

            m_MainWindow.Window.Dispatcher.Invoke(() =>
            {
                foreach (var panelType in panelTypes)
                {
                    ConnectedUserControl panel = (ConnectedUserControl)Activator.CreateInstance(panelType);
                    panel.Create(PublicContainer);
                    m_Panels.Add(panel);
                }
            });

            m_LayoutContainers = new Dictionary<Dock, ParentContainer> {{Dock.Left, new ParentContainer(m_MainWindow.Window.LeftDock)}, {Dock.Right, new ParentContainer(m_MainWindow.Window.RightDock)}, {Dock.Bottom, new ParentContainer(m_MainWindow.Window.BottomDock)}, {Dock.Top, new ParentContainer(m_MainWindow.Window.TopDock)}};
        }

        private void ApplyLayout()
        {
            foreach (ConnectedUserControl panel in m_Panels)
            {
                AddLayoutElement(panel);
            }

            Refresh();
        }
    }

    public interface ILayoutManager
    {
        void Refresh();
    }
}
