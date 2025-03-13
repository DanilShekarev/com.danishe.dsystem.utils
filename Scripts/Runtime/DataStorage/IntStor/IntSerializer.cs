using System.Globalization;
using DSystemUtils.Dynamic;
using DSystemUtils.Srialization;
using JetBrains.Annotations;
using UnityEngine;

namespace DSystemUtils.DataStorage
{
    [UsedImplicitly, ReflectionClass]
    public class IntSerializer : ISerializer<int>
    {
        public int Deserialize(string data)
        {
            if (!int.TryParse(data, out int result))
            {
                Debug.LogErrorFormat("{0} is not a valid int", data);
                return 0;
            }
            
            return result;
        }

        public string Serialize(int data)
        {
            return data.ToString(CultureInfo.InvariantCulture);
        }
    }
}