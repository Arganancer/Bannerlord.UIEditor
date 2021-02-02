using System.Collections.Generic;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidget
    {
        #region Properties

        public string Name { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }

        #endregion

        #region Constructors

        public UIEditorWidget(string _name, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Attributes = _attributes;
        }

        #endregion
    }
}
