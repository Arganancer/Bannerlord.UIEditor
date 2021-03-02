using System;
using System.Windows.Controls;

namespace Bannerlord.UIEditor.MainFrame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PanelAttribute : Attribute
    {
        public PanelAttribute(double _width, double _height, string _path, bool _isOpen = false, bool _isSelected = false)
        {
            Width = _width;
            Height = _height;
            Path = _path;
            IsOpen = _isOpen;
            IsSelected = _isSelected;
        }

        public bool IsOpen { get; set; }
        public bool IsSelected { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Path { get; set; }
    }
}
