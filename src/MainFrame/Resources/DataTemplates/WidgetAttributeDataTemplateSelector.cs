using System.Windows;
using System.Windows.Controls;
using Bannerlord.UIEditor.WidgetLibrary;

namespace Bannerlord.UIEditor.MainFrame.Resources
{
    public class WidgetAttributeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BooleanDataTemplate { get; set; }
        public DataTemplate? TextDataTemplate { get; set; }
        public DataTemplate? EnumDropDownDataTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object _item, DependencyObject _container)
        {
            if (_container is FrameworkElement element &&
                _item is UIEditorWidgetAttribute widgetAttribute)
            {
                if (widgetAttribute.Type == typeof( bool ))
                {
                    return BooleanDataTemplate;
                }

                if (widgetAttribute.Type.IsEnum)
                {
                    return EnumDropDownDataTemplate;
                }

                return TextDataTemplate;
            }

            return null;
        }
    }
}
