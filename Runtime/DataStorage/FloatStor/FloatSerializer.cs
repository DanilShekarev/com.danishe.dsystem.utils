using System.Globalization;
using DSystemUtils.Dynamic;
using DSystemUtils.Srialization;
using JetBrains.Annotations;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [UsedImplicitly, ReflectionClass]
    public class FloatSerializer : ISerializer<float>
    {
        public float Deserialize(string data)
        {
            if (!float.TryParse(data, out float result))
            {
                Debug.LogErrorFormat("{0} is not a valid float", data);
                return 0;
            }
            
            return result;
        }

        public string Serialize(float data)
        {
            return data.ToString(CultureInfo.InvariantCulture);
        }
    }
}