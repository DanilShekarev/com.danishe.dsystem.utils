using System;

namespace DSystemUtils.Dynamic
{
    public interface IDynamicData
    {
        public event Action<DynamicArguments> DataChanged;
    }
}