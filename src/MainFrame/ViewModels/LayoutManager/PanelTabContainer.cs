using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.MainFrame.Resources.Panel;

namespace Bannerlord.UIEditor.MainFrame
{
    public class PanelTabContainer : LayoutContainer<PanelControl>
    {
        private List<IPanel> Panels { get; } = new();

        public PanelTabContainer(PanelControl _layoutElement) : base(_layoutElement)
        {
            _layoutElement.Control.SizeChanged += OnSizeChanged;
        }

        public void MergeInto(PanelTabContainer _panelTabContainer)
        {
            foreach ( IPanel panel in _panelTabContainer.Panels)
            {
                _panelTabContainer.Control.RemovePanelContent(panel);
                AddPanel(panel, false);
            }
        }

        public override void Dispose()
        {
        }

        public override void Refresh()
        {
            OnDesiredWidthChanged(this, LayoutElement.DesiredWidth);
            OnDesiredHeightChanged(this, LayoutElement.DesiredHeight);
        }

        public override T? FindContainer<T>(Control _control) where T : class
        {
            return Equals(_control, Control) ? this as T : null;
        }

        public void AddPanel(IPanel _panel, bool _isSelected)
        {
            Panels.Add(_panel);
            Control.AddPanelContent(_panel, Panels.Count == 1 || _isSelected);
        }

        private void OnSizeChanged(object _sender, SizeChangedEventArgs _e)
        {
            foreach (var panel in Panels)
            {
                if (_e.WidthChanged)
                {
                    panel.SettingCategory.SetSetting("Width", _e.NewSize.Width);
                }

                if (_e.HeightChanged)
                {
                    panel.SettingCategory.SetSetting("Height", _e.NewSize.Height);
                }

                LayoutElement.DiscreteSetDesiredSize(_e.NewSize.Width, _e.NewSize.Height);
            }
        }
    }
}
