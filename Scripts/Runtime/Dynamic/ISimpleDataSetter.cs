namespace DSystemUtils.Dynamic
{
    public interface ISimpleDataSetter<T>
    {
        public T Value { get; }
        public void SetValue(T value, DynamicArguments args = null);
    }
}