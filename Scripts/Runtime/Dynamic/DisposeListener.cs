using UnityEngine;

namespace DSystemUtils.Dynamic
{
    public class DisposeListener : MonoBehaviour
    {
        public event IEventDisposable.PreDisposeDelegate Dispose;

        private void OnDestroy()
        {
            Dispose?.Invoke(gameObject, null);
        }
    }
}