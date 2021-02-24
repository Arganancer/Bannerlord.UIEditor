using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.GauntletUI;

namespace Bannerlord.UIEditor.WidgetLibrary
{
    public class UIEditorWidget
    {
        public event WidgetAttributeChangedEventHandler? PropertyChanged;
        public string Name { get; }
        public List<UIEditorWidgetAttribute> Attributes { get; }
        public List<AttributeCategory> AttributeCategories { get; }

        public bool IsReadonly
        {
            get => m_IsReadonly;
            set
            {
                if (m_IsReadonly != value)
                {
                    m_IsReadonly = value;

                    foreach (AttributeCategory attributeCategory in AttributeCategories)
                    {
                        attributeCategory.IsReadonly = m_IsReadonly;
                    }
                }
            }
        }

        public UIEditorWidgetAttribute SubscribeToAttribute( string _attributeName, Action<object?> _onPropertyChanged )
        {
            UIEditorWidgetAttribute? attribute = Attributes.FirstOrDefault(_attribute => _attribute.Name.Equals(_attributeName));
            if(attribute is null)
            {
                throw new ArgumentOutOfRangeException($"Attribute with name {_attributeName} could not be found in widget {Name}.");
            }

            attribute.PropertyChangedWithValue += ( _, _, _value ) => _onPropertyChanged(_value);
            _onPropertyChanged(attribute.Value);

            return attribute;
        }

        private bool m_IsReadonly;

        public UIEditorWidget(string _name, List<UIEditorWidgetAttribute> _attributes)
        {
            Name = _name;
            Attributes = _attributes;
            AttributeCategories = SortAttributes(_attributes);

            foreach (AttributeCategory attributeCategory in AttributeCategories)
            {
                attributeCategory.PropertyChanged += OnPropertyChanged;
            }
        }

        protected virtual void OnPropertyChanged(UIEditorWidgetAttribute _sender, string _attributeName, object? _value)
        {
            PropertyChanged?.Invoke(_sender, _attributeName, _value);
        }

        private static List<AttributeCategory> SortAttributes(IReadOnlyCollection<UIEditorWidgetAttribute> _attributes)
        {
            List<AttributeCategory> categories = new()
            {
                new AttributeCategory("Main", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "Id"),
                    _attributes.First(_a => _a.Name == "IsEnabled"),
                    _attributes.First(_a => _a.Name == "IsDisabled"),
                    _attributes.First(_a => _a.Name == "IsHidden"),
                    _attributes.First(_a => _a.Name == "IsVisible"),
                    _attributes.First(_a => _a.Name == "IsFocusable"),
                    _attributes.First(_a => _a.Name == "UpdateChildrenStates")
                }),
                new AttributeCategory("Constraints", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "VerticalAlignment"),
                    _attributes.First(_a => _a.Name == "HorizontalAlignment"),
                    _attributes.First(_a => _a.Name == "WidthSizePolicy"),
                    _attributes.First(_a => _a.Name == "HeightSizePolicy"),
                    _attributes.First(_a => _a.Name == "MaxWidth"),
                    _attributes.First(_a => _a.Name == "MaxHeight"),
                    _attributes.First(_a => _a.Name == "MinWidth"),
                    _attributes.First(_a => _a.Name == "MinHeight"),
                }),
                new AttributeCategory("Layout", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "SuggestedWidth"),
                    _attributes.First(_a => _a.Name == "SuggestedHeight"),
                    _attributes.First(_a => _a.Name == "PositionXOffset"),
                    _attributes.First(_a => _a.Name == "PositionYOffset"),
                    _attributes.First(_a => _a.Name == "MarginTop"),
                    _attributes.First(_a => _a.Name == "MarginLeft"),
                    _attributes.First(_a => _a.Name == "MarginBottom"),
                    _attributes.First(_a => _a.Name == "MarginRight"),
                    _attributes.First(_a => _a.Name == "DoNotUseCustomScaleAndChildren"),
                }),
                new AttributeCategory("Events", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "DoNotPassEventsToChildren"),
                    _attributes.First(_a => _a.Name == "DoNotAcceptEvents"),
                    _attributes.First(_a => _a.Name == "CanAcceptEvents"),
                    _attributes.First(_a => _a.Name == "HoveredCursorState"),
                    _attributes.First(_a => _a.Name == "AlternateClickEventHasSpecialEvent"),
                    _attributes.First(_a => _a.Name == "AcceptDrag"),
                    _attributes.First(_a => _a.Name == "AcceptDrop"),
                    _attributes.First(_a => _a.Name == "DropEventHandledManually"),
                    _attributes.First(_a => _a.Name == "HideOnDrag"),
                    _attributes.First(_a => _a.Name == "DragWidget")
                }),
                new AttributeCategory("Animations", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "TweenPosition"), 
                    _attributes.First(_a => _a.Name == "RestartAnimationFirstFrame")
                }),
                new AttributeCategory("Rendering", typeof( Widget ), new List<UIEditorWidgetAttribute>
                {
                    _attributes.First(_a => _a.Name == "VisualDefinition"),
                    _attributes.First(_a => _a.Name == "Sprite"),
                    _attributes.First(_a => _a.Name == "ForcePixelPerfectRenderPlacement"),
                    _attributes.First(_a => _a.Name == "ClipContents"),
                    _attributes.First(_a => _a.Name == "CircularClipEnabled"),
                    _attributes.First(_a => _a.Name == "CircularClipRadius"),
                    _attributes.First(_a => _a.Name == "IsCircularClipRadiusHalfOfWidth"),
                    _attributes.First(_a => _a.Name == "IsCircularClipRadiusHalfOfHeight"),
                    _attributes.First(_a => _a.Name == "CircularClipSmoothingRadius"),
                    _attributes.First(_a => _a.Name == "CircularClipXOffset"),
                    _attributes.First(_a => _a.Name == "CircularClipYOffset"),
                    _attributes.First(_a => _a.Name == "RenderLate"),
                    _attributes.First(_a => _a.Name == "DoNotRenderIfNotFullyInsideScissor")
                })
            };

            IEnumerable<Type> types = _attributes.Select(_a => _a.DeclaringType).Distinct().Where(_t => _t != typeof( Widget ));
            categories.AddRange(types.Select(_t => new AttributeCategory(_t.Name, _t, _attributes.Where(_a => _a.DeclaringType == _t).ToList())));

            return categories;
        }
    }
}
