using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.ButterLib.Common.Extensions;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent;
using Bannerlord.UIEditor.MainFrame.Resources.Panel;
using Bannerlord.UIEditor.MainFrame.Resources.Resizer;

namespace Bannerlord.UIEditor.MainFrame
{
    public class LayoutManager : Module, ILayoutManager
    {
        public static LayoutManager Instance { get; private set; } = null!;
        private Dictionary<Dock, ParentContainer> m_LayoutContainers = null!;

        private readonly List<ConnectedUserControl> m_Panels = new();

        private IMainWindow m_MainWindow = null!;
        private ISettingsManager m_SettingsManager = null!;
        private IGlobalEventManager m_GlobalEventManager;
        private IGlobalEvent m_DebugButtonPressedEvent;

        public void Refresh()
        {
            foreach (var layoutContainer in m_LayoutContainers)
            {
                layoutContainer.Value.Refresh();
            }
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
            m_GlobalEventManager = PublicContainer.GetModule<IGlobalEventManager>();
            m_DebugButtonPressedEvent = m_GlobalEventManager.GetEvent("DebugButtonPressed");
            m_DebugButtonPressedEvent.EventHandler += OnDebugButtonPressed;

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
            m_DebugButtonPressedEvent.EventHandler -= OnDebugButtonPressed;

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

        public T? FindContainer<T>(Control _control) where T : LayoutContainer
        {
            foreach (var container in m_LayoutContainers)
            {
                var output = container.Value.FindContainer<T>(_control);
                if (output is not null)
                {
                    return output;
                }
            }

            return null;
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

        private void OnDebugButtonPressed(object _sender, IEnumerable<object> _params)
        {
            string header = $"{"".PadLeft(15, '-')}LOGICAL LAYOUT{"".PadLeft(15, '-')}";
            Trace.WriteLine(header);
            foreach (var layoutContainer in m_LayoutContainers)
            {
                layoutContainer.Value.Refresh();
                Trace.WriteLine($"{LogicalStructureToString(0, layoutContainer.Value)}");
            }

            Trace.WriteLine("".PadLeft(header.Length, '-'));


            header = $"\n{"".PadLeft(15, '-')}ACTUAL LAYOUT{"".PadLeft(15, '-')}";
            Trace.WriteLine(header);
            foreach (var layoutContainer in m_LayoutContainers)
            {
                Trace.WriteLine($"{ActualStructureToString(0, layoutContainer.Value.LayoutElement.Control)}");
            }

            Trace.WriteLine("".PadLeft(header.Length, '-'));
        }

        private string LogicalStructureToString(int _indentation, LayoutContainer _layoutContainer)
        {
            string output = $"{(_indentation > 0 ? "\n" : "")}{string.Concat(Enumerable.Repeat("|   ", _indentation))}{Enum.GetName(typeof( Dock ), _layoutContainer.LayoutElement.CurrentDock)}:{_layoutContainer.LayoutElement.Control.Name}:{_layoutContainer.LayoutElement.Control.GetType().Name}";
            if (_layoutContainer is ParentContainer parentContainer)
            {
                if (parentContainer.PrimaryChild is not null)
                {
                    output += LogicalStructureToString(_indentation + 1, parentContainer.PrimaryChild);
                }

                if (parentContainer.SecondaryChild is not null)
                {
                    output += LogicalStructureToString(_indentation + 1, parentContainer.SecondaryChild);
                }
            }
            else if (_layoutContainer is PanelTabContainer panelTabContainer)
            {
                foreach (IPanel panel in panelTabContainer.Panels)
                {
                    output += $"\n{string.Concat(Enumerable.Repeat("|   ", _indentation + 1))}{panel.PanelName}";
                }
            }

            return output;
        }

        private string ActualStructureToString(int _indentation, FrameworkElement _frameworkElement)
        {
            string output = $"{(_indentation > 0 ? "\n" : "")}{string.Concat(Enumerable.Repeat("|   ", _indentation))}{Enum.GetName(typeof( Dock ), DockPanel.GetDock(_frameworkElement))}:{_frameworkElement.Name}:{_frameworkElement.GetType().Name}";
            var resizableControl = _frameworkElement.GetDescendantOfType<ResizableControl>()!;
            if(!resizableControl.Borders.Any())
            {
                output += " | NO BORDERS";
            }
            else
            {
                List<ResizeDirection> uninitializedBorders = new();
                foreach (var (direction, resizer) in resizableControl.Borders)
                {
                    if(resizer.LayoutElement is null)
                    {
                        uninitializedBorders.Add(direction);
                    }
                }

                if(uninitializedBorders.Any())
                {
                    output += $" | UNINITIALIZED BORDERS: {string.Join(", ",uninitializedBorders.Select(x => Enum.GetName(typeof(ResizeDirection), x)))}";
                }
                else
                {
                    output += $" | All good";
                }
            }
            if (_frameworkElement is FloatingPanelParentControl parentContainer)
            {
                var childContainers = parentContainer.ContentStackPanel.Children.OfType<FloatingPanelParentControl>();
                foreach (var childContainer in childContainers)
                {
                    output += ActualStructureToString(_indentation + 1, childContainer);
                }

                var childPanelControls = parentContainer.ContentStackPanel.Children.OfType<PanelControl>();
                foreach (var childPanel in childPanelControls)
                {
                    output += ActualStructureToString(_indentation + 1, childPanel);
                }
            }
            else if (_frameworkElement is PanelControl panelControl)
            {
                foreach (IPanel panel in panelControl.GetDescendantsOfType<IPanel>())
                {
                    output += $"\n{string.Concat(Enumerable.Repeat("|   ", _indentation + 1))}{panel.PanelName}";
                }
            }

            return output;
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

            m_LayoutContainers = new Dictionary<Dock, ParentContainer> {{Dock.Left, new ParentContainer(m_MainWindow.Window.LeftDock, PublicContainer, m_MainWindow.Window.Dispatcher)}, {Dock.Right, new ParentContainer(m_MainWindow.Window.RightDock, PublicContainer, m_MainWindow.Window.Dispatcher)}, {Dock.Bottom, new ParentContainer(m_MainWindow.Window.BottomDock, PublicContainer, m_MainWindow.Window.Dispatcher)}, {Dock.Top, new ParentContainer(m_MainWindow.Window.TopDock, PublicContainer, m_MainWindow.Window.Dispatcher)}};
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
}
