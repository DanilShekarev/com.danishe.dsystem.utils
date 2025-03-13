using DSystemUtils.Dynamic;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Data Storage/Float/Float Storage Listener")]
    public class FloatStorageListenerDEvent : FloatStorageListener
    {
        [field: SerializeField] public DEvent<float> OnValueChange { get; private set; }

        protected override void OnValueChanged(DynamicArguments obj)
        {
            base.OnValueChanged(obj);
            
            OnValueChange?.Invoke(Storage.Value);
        }
    }
}