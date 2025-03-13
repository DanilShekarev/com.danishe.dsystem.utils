using DSystemUtils.Dynamic;
using UnityEngine;
using UnityEngine.Events;

namespace DSystemUtils.DataStorage
{
    [AddComponentMenu("Utils/Int Checker")]
    public partial class IntChecker : MonoBehaviour
    {
        [field: SerializeField] public UnityEvent<bool> OnStateChanged { get; private set; }
        
        [SerializeField] private ValueProvider<int> targetValue;

        public virtual void CheckMoreOrEqual(int value)
        {
            OnStateChanged?.Invoke(value >= targetValue.GetValue());
        }
    }
}