namespace DSystemUtils.Dynamic.CDS
{
    public interface IDataController<in T> : IDataController where T : IDynamicData
    {
        void IDataController.AttachInternal(IDynamicData data)
        {
            AttachInternal((T)data);
        }
        
        void IDataController.DetachInternal(IDynamicData data)
        {
            DetachInternal((T)data);
        }
        
        internal void AttachInternal(T data);
        internal void DetachInternal(T data);
    }

    public interface IDataController
    {
        internal void AttachInternal(IDynamicData data);
        internal void DetachInternal(IDynamicData data);
    }
}