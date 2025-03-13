using DSystem;

namespace DSystemUtils.Dynamic.DAS
{
    [Listener]
    // ReSharper disable once InconsistentNaming
    public interface IDASListener
    {
        internal void OnDataAttachment(object target, IDynamicData data);
        internal void OnDataDisconnected(object target, IDynamicData data);
    }
}