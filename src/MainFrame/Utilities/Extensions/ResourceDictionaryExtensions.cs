using System.Linq;
using System.Windows;

namespace Bannerlord.UIEditor.MainFrame
{
    public static class ResourceDictionaryExtensions
    {
        public static ResourceDictionary? FindMergedDictionaryWithKeys(this ResourceDictionary _instance, params object[] _keys)
        {
            if (_keys.All(_instance.Keys.OfType<object>().Contains))
            {
                return _instance;
            }

            foreach (ResourceDictionary child in _instance.MergedDictionaries)
            {
                ResourceDictionary? output = child.FindMergedDictionaryWithKeys(_keys);
                if (output is not null)
                {
                    return output;
                }
            }

            return null;
        }
    }
}
