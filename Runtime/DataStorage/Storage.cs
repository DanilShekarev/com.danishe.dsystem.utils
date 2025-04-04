using System;
using DSystemUtils.Dynamic;
using DSystemUtils.Srialization;
using DSystemUtils.Utils;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    public sealed class Storage<T>
    {
        public event Action<DynamicArguments> ValueChanged;
        
        public T Value { get; private set; }

        private readonly ISerializer<T> _serializer;
        private readonly string _key;

        public Storage(string key, ISerializer<T> serializer, string defaultData = null)
        {
            _key = key;
            _serializer = serializer;
            Load(defaultData);
            if (Value is IDynamicData dynamicData)
            {
                dynamicData.DataChanged += OnDataChanged;
            }
        }

        private void OnDataChanged(DynamicArguments args = null)
        {
            Save(args);
        }

        private void Save(DynamicArguments args = null)
        {
            PlayerPrefs.SetString(_key, _serializer.Serialize(Value));
            ValueChanged?.Invoke(args);
        }

        private void Load(string defaultData = null)
        {
            string data = PlayerPrefs.GetString(_key, defaultData);
            ParseData(data);
        }

        private void ParseData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            SetValue(_serializer.Deserialize(data));
        }

        public void SetValue(T value, DynamicArguments args = null)
        {
            OldValue<T> oldValue = new OldValue<T>(Value);
            args ??= new DynamicArguments();
            args.AddArgument(oldValue);
            
            Value = value;
            Save(args);
        }

        public static implicit operator T(Storage<T> storage) => storage.Value;
    }
}