using System;

namespace Bannerlord.UIEditor.Core
{
    public interface ISettingCategory
    {
        /// <summary>
        /// Returns the value of the setting that corresponds to the given <paramref name="_name"/>.<br/>
        /// If the setting is not set, creates the setting and sets its value to <paramref name="_defaultValue"/>, then returns <paramref name="_defaultValue"/>.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
        T? GetSetting<T>(string _name, T? _defaultValue);

        /// <summary>
        /// Returns the value of the setting that corresponds to the given <paramref name="_name"/>.<br/>
        /// Returns null if the setting is not set.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
        T? GetSetting<T>(string _name);

        /// <summary>
        /// If the setting exists, updates its value, else creates the setting and sets its value.<br/>
        /// <exception cref="InvalidCastException"> <see cref="InvalidCastException"/>: Thrown if the setting exists and can't cast the value to <typeparam name="T">T</typeparam></exception>
        /// </summary>
        void SetSetting<T>(string _name, T? _setting);
    }
}
