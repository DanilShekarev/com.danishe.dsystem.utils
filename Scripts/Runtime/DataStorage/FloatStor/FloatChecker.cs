using DSystemUtils.Dynamic;
using UnityEngine;
using UnityEngine.Events;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Utils/Float Checker")]
    public partial class FloatChecker : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<bool> OnStateChanged { get; private set; }
        
        [SerializeField] private ValueProvider<float> targetValue;

        public virtual void CheckMoreOrEqual(float value)
        {
            OnStateChanged?.Invoke(value >= targetValue.GetValue());
        }
    }
}