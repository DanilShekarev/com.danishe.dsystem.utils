namespace DSystemUtils.DataStorage
{
    public static partial class IntStorage
    {
        public struct AddedVal
        {
            public int AddedValue;
        }
        
        public static void Add(this Storage<int> storage, int value)
        {
            storage.SetValue(storage.Value + value, new (new AddedVal {AddedValue = value}));
        }
        
        public static void Remove(this Storage<int> storage, int value)
        {
            storage.SetValue(storage.Value - value, new (new AddedVal {AddedValue = -value}));
        }
    }
}