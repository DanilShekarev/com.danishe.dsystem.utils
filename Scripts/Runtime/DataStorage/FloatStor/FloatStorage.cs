namespace DSystemUtils.DataStorage
{
    public static partial class FloatStorage
    {
        public struct AddedVal
        {
            public float AddedValue;
        }
        
        public static void Add(this Storage<float> storage, float value)
        {
            storage.SetValue(storage.Value + value, new (new AddedVal {AddedValue = value}));
        }
        
        public static void Remove(this Storage<float> storage, float value)
        {
            storage.SetValue(storage.Value - value, new (new AddedVal {AddedValue = -value}));
        }
    }
}