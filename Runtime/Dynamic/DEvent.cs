using System;
using UnityEngine;

namespace DSystemUtils.Dynamic
{
    [Serializable]
    public class DEvent<T>
    {
        [SerializeField] private EventData<T>[] events;

        public void Invoke(T value)
        {
            foreach (var ev in events)
            {
                try
                {
                    ev.Invoke(value);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}