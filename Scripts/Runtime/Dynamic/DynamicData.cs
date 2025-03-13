using System;
using UnityEngine;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public class DynamicData<T> : IDynamicData, ISimpleDataSetter<T>, IEventDisposable
    {
        public event IEventDisposable.PreDisposeDelegate PreDispose;
        public event Action<DynamicArguments> DataChanged;
        
        [field: SerializeField] public T Value { get; private set; }

        public virtual void SetValue(T value, DynamicArguments args = null)
        {
            Value = value;
            DataChanged?.Invoke(args);
        }
        
        public static implicit operator T(DynamicData<T> data) => data.Value;
        
        public void Dispose(DynamicArguments arguments = null)
        {
            PreDispose?.Invoke(this, arguments);
        }
    }
}