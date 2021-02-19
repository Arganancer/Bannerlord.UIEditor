using System;
using System.Collections.Concurrent;

namespace Bannerlord.UIEditor.Core.DataRepo
{
    public class DataRepository : Module, IDataRepository
    {
        private ConcurrentDictionary<string, object> m_Data = null!;

        public bool TryGetData<T>(string _name, out T? _data)
        {
            if (m_Data.TryGetValue(_name, out object data))
            {
                _data = (T)data;
                return true;
            }

            _data = default;
            return false;
        }

        public void AddOrUpdateData(string _name, object _data)
        {
            m_Data.AddOrUpdate(_name, _data, (_, _) => _data);
        }

        /// <inheritdoc />
        public T GetDataWithDefault<T>(string _name, Func<T> _addDefaultIfNotExist)
        {
            if (!TryGetData(_name, out T? data))
            {
                data = _addDefaultIfNotExist();
                AddOrUpdateData(_name, data!);
            }

            return data!;
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);

            m_Data = new ConcurrentDictionary<string, object>();

            RegisterModule<IDataRepository>();
        }
    }
}
