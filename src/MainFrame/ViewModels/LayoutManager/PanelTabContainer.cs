using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Bannerlord.UIEditor.Core;
using Bannerlord.UIEditor.MainFrame.Resources.FloatingPanelParent;
using Bannerlord.UIEditor.MainFrame.Resources.Panel;

namespace Bannerlord.UIEditor.MainFrame
{
    public class PanelTabContainer : LayoutContainer<PanelControl>
    {
        public IReadOnlyList<IPanel> Panels => m_Panels;
        private List<IPanel> m_Panels = new();

        public PanelTabContainer(PanelControl _layoutElement, IPublicContainer _publicContainer, Dispatcher _dispatcher) : base(_layoutElement, _publicContainer, _dispatcher)
        {
            _layoutElement.Control.SizeChanged += OnSizeChanged;
            _layoutElement.MovePanel += OnMovePanel;
        }

        public override void Dispose()
        {
        }

        public override void Refresh()
        {
            Dispatcher.Invoke(() =>
            {
                LayoutElement.RefreshResizerBorders(GetBorders().ToArray());
                OnDesiredWidthChanged(this, LayoutElement.DesiredWidth);
                OnDesiredHeightChanged(this, LayoutElement.DesiredHeight);
            });
        }

        public override T? FindContainer<T>(Control _control) where T : class
        {
            return Equals(_control, Control) ? this as T : null;
        }

        public void MergeInto(PanelTabContainer _panelTabContainer)
        {
            foreach (IPanel panel in _panelTabContainer.m_Panels)
            {
                _panelTabContainer.Control.RemovePanelContent(panel);
                AddPanel(panel, false);
            }
        }

        public void AddPanel(IPanel _panel, bool _isSelected, int _targetIndex = -1)
        {
            m_Panels.Add(_panel);
            Control.AddPanelContent(_panel, m_Panels.Count == 1 || _isSelected, _targetIndex);
        }

        public void RemovePanel(IPanel _panel)
        {
            m_Panels.Remove(_panel);
            Control.RemovePanelContent(_panel);
        }

        private void OnMovePanel(PanelControl _target, PanelControl _origin, IPanel? _panel, DropTarget _dropTarget, int _targetIndex)
        {
            var originalPanelTabContainer = LayoutManager.Instance.FindContainer<PanelTabContainer>(_origin);
            if (originalPanelTabContainer is null)
            {
                return;
            }

            ParentContainer originParent = originalPanelTabContainer.Parent!;
            ParentContainer targetParent = Parent!;
            PanelTabContainer panelToMove = null!;
            if (_panel is not null && _origin.TabControl.Items.Count > 1)
            {
                originalPanelTabContainer.RemovePanel(_panel);
                Dispatcher.Invoke(() =>
                {
                    PanelControl panelControl = new();
                    panelControl.Create(PublicContainer);
                    panelControl.Load();
                    panelToMove = new PanelTabContainer(panelControl, PublicContainer, Dispatcher);
                    panelToMove.AddPanel(_panel, true);
                    panelToMove.LayoutElement.DiscreteSetDesiredSize(_origin.DesiredWidth, _origin.DesiredHeight);
                });

                originParent.Refresh();
            }
            else
            {
                panelToMove = originalPanelTabContainer;
                originParent.RemoveChild(originalPanelTabContainer);
            }

            switch (_dropTarget)
            {
                case DropTarget.Left:
                    PerformPanelControlMove(targetParent, panelToMove, this, Orientation.Horizontal, Dock);
                    break;
                case DropTarget.Right:
                    PerformPanelControlMove(targetParent, this, panelToMove, Orientation.Horizontal, Dock);
                    break;
                case DropTarget.Top:
                    PerformPanelControlMove(targetParent, panelToMove, this, Orientation.Vertical, Dock);
                    break;
                case DropTarget.Bottom:
                    PerformPanelControlMove(targetParent, this, panelToMove, Orientation.Vertical, Dock);
                    break;
                case DropTarget.Center:
                    if (_panel is null)
                    {
                        MergeInto(panelToMove);
                    }
                    else
                    {
                        AddPanel(_panel, true, _targetIndex);
                    }
                    break;
            }

            targetParent.Refresh();
        }

        private void PerformPanelControlMove(ParentContainer _targetParent,
            LayoutContainer _primaryChild,
            LayoutContainer _secondaryChild,
            Orientation _orientation,
            Dock _originalDock)
        {
            if (IsOnlyChild)
            {
                if (!_primaryChild.Equals(_targetParent.PrimaryChild))
                {
                    if (_primaryChild.Equals(_targetParent.SecondaryChild))
                    {
                        _targetParent.RemoveChild(_primaryChild);
                    }

                    _targetParent.PrimaryChild = _primaryChild;
                }

                if (!_secondaryChild.Equals(_targetParent.SecondaryChild))
                {
                    if (_secondaryChild.Equals(_targetParent.PrimaryChild))
                    {
                        _targetParent.RemoveChild(_secondaryChild);
                    }

                    _targetParent.SecondaryChild = _secondaryChild;
                }

                _targetParent.Orientation = _orientation;
            }
            else
            {
                _targetParent.RemoveChild(this);
                ParentContainer newParent = new(new FloatingPanelParentControl(), PublicContainer, Dispatcher) {PrimaryChild = _primaryChild, SecondaryChild = _secondaryChild, Orientation = _orientation};
                _targetParent.AddChild(newParent, _originalDock);
                newParent.Refresh();
            }
        }

        private void OnSizeChanged(object _sender, SizeChangedEventArgs _e)
        {
            foreach (var panel in m_Panels)
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
