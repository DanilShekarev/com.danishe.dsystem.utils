using DSystem;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Data Storage/Float/Float Storage Changer")]
    public partial class FloatStorageChanger : DBehaviour
    {
        [SerializeField] private StorageKey storageKey;
        
        [Inject] private DataStorageSystem _dataStorageSystem;
        
        private Storage<float> _storage;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            _storage = _dataStorageSystem.GetStorage<float>(storageKey);
        }

        public void Add(float amount)
        {
            _storage.Add(amount);
        }

        public void Remove(float amount)
        {
            _storage.Remove(amount);
        }
    }
}