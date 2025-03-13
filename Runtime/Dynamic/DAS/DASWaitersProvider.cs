using System.Reflection;
using DSystem;
using JetBrains.Annotations;

namespace DSystemUtils.Dynamic.DAS
{
    [UsedImplicitly, AutoRegistry]
    public class DASWaitersProvider : DSystemBase, IDASListener
    {
        void IDASListener.OnDataAttachment(object target, IDynamicData data)
        {
            var listenerType = typeof(IDASDataWaiter<>);
            var dataType = data.GetType();
            listenerType = listenerType.MakeGenericType(dataType);
            var methods = listenerType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var invokeMethod = methods[0];
            var dAction = MainInjector.Instance.GetDAction(listenerType, false);
            dAction?.Invoke(listener =>
            {
                IDASObjectTargetProvider targetProvider = listener as IDASObjectTargetProvider;
                if (targetProvider.TargetObjectData != target) return;
                invokeMethod.Invoke(listener, new object[] { data });
            });
        }

        void IDASListener.OnDataDisconnected(object target, IDynamicData data)
        {
            
        }
    }
}