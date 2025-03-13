using System;

namespace DSystemUtils.Dynamic
{
    public interface IEventDisposable : IDisposable
    {
        public delegate void PreDisposeDelegate(object sender, DynamicArguments arguments);
        public event PreDisposeDelegate PreDispose;
        
        public void Dispose(DynamicArguments arguments = null);
        
        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}