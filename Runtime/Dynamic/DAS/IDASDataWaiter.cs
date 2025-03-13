using DSystem;

namespace DSystemUtils.Dynamic.DAS
{
    [Listener]
    public interface IDASDataWaiter<in T> : IDASObjectTargetProvider where T : IDynamicData
    {
        internal void OnDataAttached(T data);
    }
}