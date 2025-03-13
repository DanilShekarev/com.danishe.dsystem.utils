using System;
using System.Collections.Generic;
using DSystem;
using JetBrains.Annotations;

namespace DSystemUtils.Srialization
{
    [AutoRegistry, UsedImplicitly]
    public class SerializationsSystem : DSystemBase
    {
        private readonly Dictionary<Type, object> _serializations = new ();

        public ISerializer<T> GetSerializer<T>()
        {
            return GetSerializer(typeof(ISerializer<T>)) as ISerializer<T>;
        }

        public object GetSerializer(Type type)
        {
            if (_serializations.TryGetValue(type, out var serializer))
            {
                return serializer;
            }
            var inst = Activator.CreateInstance(type);
            _serializations.Add(type, inst);
            return inst;
        }
    }
}