using DSystem;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Data Storage/Int/Int Storage Changer")]
    public partial class IntStorageChanger : DBehaviour
    {
        [SerializeField] private StorageKey storageKey;
        
        [Inject] private DataStorageSystem _dataStorageSystem;
        
        private Storage<int> _storage;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            _storage = _dataStorageSystem.GetStorage<int>(storageKey);
        }

        public void Add(int amount)
        {
            _storage.Add(amount);
        }

        public void Remove(int amount)
        {
            _storage.Remove(amount);
        }
    }
}