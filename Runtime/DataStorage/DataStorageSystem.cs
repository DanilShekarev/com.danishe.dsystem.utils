using System;
using System.Collections.Generic;
using System.Linq;
using DSystem;
using DSystemUtils.Srialization;
using JetBrains.Annotations;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [AutoRegistry, UsedImplicitly]
    public class DataStorageSystem : DSystemBase, IInitializable
    {
        [Inject] private SerializationsSystem _serializationSystem;
        
        private readonly Dictionary<string, object> _storage = new();
        
        void IInitializable.Initialize()
        {
            var storagesKeys = Resources.LoadAll<StorageKey>("");
            foreach (var storageKey in storagesKeys)
            {
                CreateStorage(storageKey.Key, storageKey.DataSerializer.Type, storageKey.DefaultValue);
            }
        }

        public Storage<T> GetStorage<T>(StorageKey key)
            => GetStorage<T>(key.Key);
        
        public Storage<T> GetStorage<T>(string key)
        {
            if (_storage.TryGetValue(key, out var result))
            {
                return (Storage<T>)result;
            }

            return null;
        }

        public object CreateStorage(string key, Type serializerType, string defaultValue = null)
        {
            if (serializerType == null)
                return null;
            var firstInterface = serializerType.GetInterfaces().FirstOrDefault(x => x.IsGenericType);
            if (firstInterface != null)
            {
                var type = firstInterface.GenericTypeArguments[0];
                var storageType = typeof(Storage<>).MakeGenericType(type);
                var storageInstance = Activator.CreateInstance(
                    storageType, key, _serializationSystem.GetSerializer(serializerType), defaultValue);
                _storage.Add(key, storageInstance);
                return storageInstance;
            }

            return null;
        }
    }
}