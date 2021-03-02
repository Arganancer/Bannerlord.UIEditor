using System;

namespace Bannerlord.UIEditor.Core
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] _array, int _startIndex, int _length = 0)
        {
            int maxLength = _array.Length - _startIndex;
            int length = _length == 0 ? maxLength : Math.Min(_length, maxLength);
            T[] result = new T[length];
            Array.Copy(_array, _startIndex, result, 0, length);
            return result;
        }
    }
}
