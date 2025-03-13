using DSystemUtils.Dynamic.DI;
using DSystemUtils.Srialization;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [CreateAssetMenu(fileName = "StorageKey", menuName = "Storages/Key", order = 0)]
    public class StorageKey : ScriptableObject
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public string DefaultValue { get; private set; }

        [field: SerializeField] public TypeContainer<ISerializer> DataSerializer { get; private set; }
    }
}