using System;
using UnityEngine;

namespace DSystemUtils.Dynamic.DI
{
    [Serializable]
    public class TypeContainer<TInterface> : ISerializationCallbackReceiver where TInterface : class
    {
        [NonSerialized]
        private Type _type;

        [SerializeField]
        private string _typeName;

        public Type Type
        {
            get => _type;
            set => _type = typeof(TInterface).IsInterface && value != null && typeof(TInterface).IsAssignableFrom(value) 
                ? value 
                : null;
        }

        public void OnBeforeSerialize()
        {
            _typeName = _type?.AssemblyQualifiedName;
        }

        public void OnAfterDeserialize()
        {
            _type = !string.IsNullOrEmpty(_typeName) ? Type.GetType(_typeName) : null;
        }
    }
}