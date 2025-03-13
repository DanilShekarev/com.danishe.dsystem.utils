using DSystem;
using DSystemUtils.Dynamic;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    public abstract class IntStorageListener : DBehaviour
    {
        [SerializeField] private StorageKey storageKey;
        
        [Inject] private DataStorageSystem _dataStorageSystem;
        
        protected Storage<int> Storage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            Storage = _dataStorageSystem.GetStorage<int>(storageKey);
            
            Storage.ValueChanged += OnValueChanged;
        }

        protected virtual void Start()
        {
            OnValueChanged(null);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            Storage.ValueChanged -= OnValueChanged;
        }

        protected virtual void OnValueChanged(DynamicArguments obj)
        {
            
        }
    }
}