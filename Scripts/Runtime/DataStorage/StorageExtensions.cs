using DSystem;

namespace DSystemUtils.DataStorage
{
    public static class StorageExtensions
    {
        public static DataStorageSystem DSS
        {
            get
            {
                if (_dss == null)
                    MainInjector.Instance.TryGetSystem(out _dss);
                return _dss;
            }
        }

        private static DataStorageSystem _dss;

        public static Storage<T> GetStorage<T>(this StorageKey storageKey)
        {
            return DSS.GetStorage<T>(storageKey);
        }
    }
}