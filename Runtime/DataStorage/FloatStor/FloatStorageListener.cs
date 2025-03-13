using DSystem;
using DSystemUtils.Dynamic;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    public abstract class FloatStorageListener : DBehaviour
    {
        [SerializeField] private StorageKey storageKey;
        
        [Inject] private DataStorageSystem _dataStorageSystem;
        
        protected Storage<float> Storage { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            Storage = _dataStorageSystem.GetStorage<float>(storageKey);
            
            Storage.ValueChanged += OnValueChanged;
        }

        private void Start()
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