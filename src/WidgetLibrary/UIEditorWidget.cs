using System.Collections.Generic;
using System.Reflection;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidget
    {
        #region Public Properties

        public string Name { get; }
        public Assembly Owner { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }

        #endregion

        #region Constructors

        public UIEditorWidget(string _name, Assembly _owner, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Owner = _owner;
            Attributes = _attributes;
        }

        #endregion
    }
}
