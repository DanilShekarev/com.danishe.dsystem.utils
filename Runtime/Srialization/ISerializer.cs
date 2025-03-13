namespace DSystemUtils.Srialization
{
    public interface ISerializer<T> : ISerializer
    {
        public T Deserialize(string data);
        public string Serialize(T data);

        object ISerializer.Deserialize(string data)
        {
            return Deserialize(data);
        }

        string ISerializer.Serialize(object data)
        {
            return Serialize((T)data);
        }
    }

    public interface ISerializer
    {
        public object Deserialize(string data);
        public string Serialize(object data);
    }
}