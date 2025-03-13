using DSystemUtils.Dynamic;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Data Storage/Int/Int Storage Listener")]
    public class IntStorageListenerDEvent : IntStorageListener
    {
        [field: SerializeField] public DEvent<int> OnValueChange { get; private set; }

        protected override void OnValueChanged(DynamicArguments obj)
        {
            base.OnValueChanged(obj);
            
            OnValueChange?.Invoke(Storage.Value);
        }
    }
}