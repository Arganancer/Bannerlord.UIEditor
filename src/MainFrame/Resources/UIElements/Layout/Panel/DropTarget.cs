using System;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame.Resources.Panel
{
    public enum DropTarget
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }

    public static class DropTargetExtensions
    {
        public static Dock? ToDock(this DropTarget _dropTarget)
        {
            return _dropTarget switch
            {
                DropTarget.Left => Dock.Left,
                DropTarget.Right => Dock.Right,
                DropTarget.Top => Dock.Top,
                DropTarget.Bottom => Dock.Bottom,
                DropTarget.Center => null,
                _ => throw new ArgumentOutOfRangeException(nameof( _dropTarget ), _dropTarget, null)
            };
        }

        public static Orientation? GetOrientation(this DropTarget _dropTarget)
        {
            return _dropTarget switch
            {
                DropTarget.Left => Orientation.Horizontal,
                DropTarget.Right => Orientation.Horizontal,
                DropTarget.Top => Orientation.Vertical,
                DropTarget.Bottom => Orientation.Vertical,
                DropTarget.Center => null,
                _ => throw new ArgumentOutOfRangeException(nameof( _dropTarget ), _dropTarget, null)
            };
        }

        public static Orientation GetOrientation(this Dock _dock)
        {
            return _dock switch
            {
                Dock.Left => Orientation.Horizontal,
                Dock.Right => Orientation.Horizontal,
                Dock.Top => Orientation.Vertical,
                Dock.Bottom => Orientation.Vertical,
                _ => throw new ArgumentOutOfRangeException(nameof( _dock ), _dock, null)
            };
        }

        public static bool? IsPrimary(this DropTarget _dropTarget)
        {
            return _dropTarget switch
            {
                DropTarget.Left => true,
                DropTarget.Right => false,
                DropTarget.Top => true,
                DropTarget.Bottom => false,
                DropTarget.Center => null,
                _ => throw new ArgumentOutOfRangeException(nameof( _dropTarget ), _dropTarget, null)
            };
        }

        public static bool IsPrimary(this Dock _dock)
        {
            return _dock switch
            {
                Dock.Left => true,
                Dock.Right => false,
                Dock.Top => true,
                Dock.Bottom => false,
                _ => throw new ArgumentOutOfRangeException(nameof( _dock ), _dock, null)
            };
        }

        public static Dock GetPrimaryDock(this Orientation _orientation)
        {
            return _orientation switch
            {
                Orientation.Horizontal => Dock.Left,
                Orientation.Vertical => Dock.Top,
                _ => throw new ArgumentOutOfRangeException(nameof( _orientation ), _orientation, null)
            };
        }
        public static Dock GetSecondaryDock(this Orientation _orientation)
        {
            return _orientation switch
            {
                Orientation.Horizontal => Dock.Right,
                Orientation.Vertical => Dock.Bottom,
                _ => throw new ArgumentOutOfRangeException(nameof(_orientation), _orientation, null)
            };
        }
    }
}
