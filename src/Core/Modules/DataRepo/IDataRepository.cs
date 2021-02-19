using System;

namespace Bannerlord.UIEditor.Core.DataRepo
{
    public interface IDataRepository
    {
        bool TryGetData<T>(string _name, out T? _data);
        void AddOrUpdateData(string _name, object _data);

        /// <summary>
        /// Gets the object if it exists, else creates and inserts the object using <paramref name="_addDefaultIfNotExist"/>,
        /// then returns the created value.
        /// </summary>
        T GetDataWithDefault<T>(string _name, Func<T> _addDefaultIfNotExist);
    }
}
