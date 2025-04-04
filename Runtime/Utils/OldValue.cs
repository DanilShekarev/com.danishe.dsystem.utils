namespace DSystemUtils.Utils
{
    public readonly struct OldValue<T>
    {
        public readonly T Value;

        public OldValue(T value)
        {
            Value = value;
        }
    }
}