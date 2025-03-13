using System;
using System.Collections.Generic;
using DSystem;
using DSystemUtils.Reflection;
using JetBrains.Annotations;

namespace DSystemUtils.Dynamic.DAS
{
    [AutoRegistry, UsedImplicitly]
    public class DataAttachmentSystem : DSystemBase
    {
        private readonly Dictionary<object, Dictionary<Type, IDynamicData>> _dynamicData = new();

        public bool AttachDynamicData([NotNull] object target, [NotNull] IDynamicData dynamicData)
        {
            if (!_dynamicData.TryGetValue(target, out var objectDic))
            {
                objectDic = new Dictionary<Type, IDynamicData>();
                _dynamicData.Add(target, objectDic);
                target.TryDisposeSubscribe(OnPreDisposeTarget);
            }
            var result = objectDic.TryAdd(dynamicData.GetType(), dynamicData);
            if (result)
            {
                dynamicData.TryDisposeSubscribe(OnPreDisposeData(target));
                GetDAction<IDASListener>().Invoke(l => l.OnDataAttachment(target, dynamicData));   
            }
            return result;
        }
        
        public bool TryGetDynamicData<T>([NotNull] object target, out T dynamicData) where T : class, IDynamicData
        {
            if (!_dynamicData.TryGetValue(target, out var dic))
            {
                dynamicData = null;
                return false;
            }

            var result = dic.TryGetValue(typeof(T), out var data);
            if (result)
                dynamicData = (T)data;
            else
                dynamicData = null;
            
            return result;
        }

        public void DetachAllDynamicData([NotNull] object target)
        {
            if (!_dynamicData.Remove(target, out var dic))
                return;

            foreach (var pair in dic)
            {
                if (pair.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            dic.Clear();
        }

        public void DetachDynamicData([NotNull] object target, [NotNull] IDynamicData dynamicData)
        {
            DetachDynamicData(target, dynamicData.GetType());
        }

        public void DetachDynamicData<T>([NotNull] object target)
        {
            DetachDynamicData(target, typeof(T));
        }

        public void DetachDynamicData([NotNull] object target, [NotNull] Type type)
        {
            if (!_dynamicData.TryGetValue(target, out var dic))
                return;
            
            dic.Remove(type, out var val);
            val.TryDisposeUnsubscribe(OnPreDisposeData(target));
        }

        private void OnPreDisposeTarget(object sender, DynamicArguments args)
        {
            DetachAllDynamicData(sender);
        }

        private IEventDisposable.PreDisposeDelegate OnPreDisposeData(object target)
        {
            return (sender, args) =>
            {
                DetachDynamicData(target, sender as IDynamicData);
            };
        }
    }
}