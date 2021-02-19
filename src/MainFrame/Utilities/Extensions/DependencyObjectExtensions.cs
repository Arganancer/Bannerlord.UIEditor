using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Bannerlord.UIEditor.MainFrame
{
    public static class DependencyObjectExtensions
    {
        public static T? GetVisualAncestorOfType<T>(this DependencyObject _instance) where T : class
        {
            var currentObject = _instance;
            do
            {
                currentObject = VisualTreeHelper.GetParent(currentObject);
                if (currentObject is T castParent)
                {
                    return castParent;
                }
            } while (currentObject is not null);

            return null;
        }

        public static IEnumerable<T>? GetVisualAncestorsOfType<T>(this DependencyObject _instance) where T : class
        {
            var currentObject = _instance;
            do
            {
                currentObject = VisualTreeHelper.GetParent(currentObject);
                if (currentObject is T castParent)
                {
                    yield return castParent;
                }
            } while (currentObject is not null);
        }

        /// <summary>
        /// Searches for the first visual child of type <typeparam name="T"/> in <paramref name="_instance"/> using <see cref="VisualTreeHelper"/>.<br/>
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// Returns null if no child of the specified type is found.<br/>
        /// See the example section for more info.
        /// </summary>
        /// 
        /// <example>
        /// If looking for an item of type List and _expandChildren is set to true:
        /// <code>
        /// <Root>
        ///     <Object>
        ///         <List/> <!-- Object will be expanded before passing to it's sibling, so this Visual object will be returned. -->
        ///     </Object>
        ///     <List/>
        /// </Root>
        /// </code>
        /// </example>
        public static T? GetVisualDescendantOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(_instance); i++)
            {
                var child = VisualTreeHelper.GetChild(_instance, i);
                if (child is T castChild)
                {
                    return castChild;
                }

                if (_expandChildren)
                {
                    child.GetVisualDescendantOfType<T>(_expandChildren);
                }
            }

            return default;
        }

        /// <summary>
        /// Searches for all visual children of type <typeparam name="T"/> in <paramref name="_instance"/> using <see cref="VisualTreeHelper"/>.<br/>
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// See the example section for more info.
        /// </summary>
        ///
        /// <example>
        /// If looking for items of type List and _expandChildren is set to true:
        /// <code>
        /// <Root>
        ///     <Object>
        ///         <List/> <!-- Will be returned first. -->
        ///     </Object>
        ///     <List/> <!-- Will be returned second. -->
        /// </Root>
        /// </code>
        /// </example>
        public static IEnumerable<T> GetVisualDescendantsOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(_instance); i++)
            {
                var child = VisualTreeHelper.GetChild(_instance, i);
                if (child is T castChild)
                {
                    yield return castChild;
                }

                if (_expandChildren)
                {
                    foreach (var childOfChild in child.GetVisualDescendantsOfType<T>(_expandChildren))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Searches for the first logical child of type <typeparam name="T"/> in <paramref name="_instance"/> using <see cref="LogicalTreeHelper"/>.<br/>
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// Returns null if no child of the specified type is found.<br/>
        /// <paramref name="_expandChildren"/> behavior works the same as <see cref="GetVisualDescendantOfType{T}"/>. See it's example section for more info.
        /// </summary>
        public static T? GetLogicalDescendantOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(_instance))
            {
                if (child is T castChild)
                {
                    return castChild;
                }

                if (_expandChildren && child is DependencyObject dependencyObject)
                {
                    dependencyObject.GetLogicalDescendantOfType<T>(_expandChildren);
                }
            }

            return default;
        }

        /// <summary>
        /// Searches for all logical children of type <typeparam name="T"/> in <paramref name="_instance"/> using <see cref="LogicalTreeHelper"/>.<br/>
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// <paramref name="_expandChildren"/> behavior works the same as <see cref="GetVisualDescendantsOfType{T}"/>. See it's example section for more info.
        /// </summary>
        public static IEnumerable<T> GetLogicalDescendantsOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(_instance))
            {
                if (child is T castChild)
                {
                    yield return castChild;
                }

                if (_expandChildren && child is DependencyObject dependencyObject)
                {
                    foreach (var childOfChild in dependencyObject.GetLogicalDescendantsOfType<T>(_expandChildren))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Searches for the first visual (using <see cref="VisualTreeHelper"/>) or logical (using <see cref="LogicalTreeHelper"/>) child of type <typeparam name="T"/>
        /// in <paramref name="_instance"/>. Will search the VisualTree before searching the LogicalTree.<br/>
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// Returns null if no child of the specified type is found.<br/>
        /// <paramref name="_expandChildren"/> behavior works the same as <see cref="GetVisualDescendantOfType{T}"/>. See it's example section for more info.
        /// </summary>
        public static T? GetDescendantOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            return _instance.GetVisualDescendantOfType<T>(_expandChildren) ?? _instance.GetLogicalDescendantOfType<T>(_expandChildren);
        }

        /// <summary>
        /// Searches for all visual (using <see cref="VisualTreeHelper"/>) and logical (using <see cref="LogicalTreeHelper"/>) children of type <typeparam name="T"/>
        /// in <paramref name="_instance"/>. Will search the VisualTree before searching the LogicalTree.
        /// If <paramref name="_expandChildren"/> is true, will look through children recursively. Children are fully expanded before passing to the next top-level child.<br/>
        /// If an item exists both in the Visual and Logical tree, it will only be included in the output a single time.<br/>
        /// <paramref name="_expandChildren"/> behavior works the same as <see cref="GetVisualDescendantsOfType{T}"/>. See it's example section for more info.
        /// </summary>
        public static IEnumerable<T> GetDescendantsOfType<T>(this DependencyObject _instance, bool _expandChildren = true)
        {
            return _instance.GetVisualDescendantsOfType<T>(_expandChildren).Union(_instance.GetLogicalDescendantsOfType<T>(_expandChildren));
        }
    }
}
