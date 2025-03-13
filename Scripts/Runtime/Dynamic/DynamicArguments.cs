using System;
using System.Collections.Generic;
using System.Linq;

namespace DSystemUtils.Dynamic
{
    public class DynamicArguments : EventArgs
    {
        private readonly Dictionary<Type, object> _args;

        public DynamicArguments(params object[] args)
        {
            _args = new Dictionary<Type, object>();
            foreach (var arg in args)
            {
                _args.Add(arg.GetType(), arg);
            }
        }

        public void AddArgument<T>(T arg)
        {
            _args[typeof(T)] = arg;
        }

        public void RemoveArgument<T>(T arg)
        {
            _args.Remove(typeof(T));
        }

        public void Clear()
        {
            _args.Clear();
        }

        public bool TryGetArgument<T>(out T argument)
        {
            if (!_args.TryGetValue(typeof(T), out var value))
            {
                argument = default;
                return false;
            }
            argument = (T)value;
            return true;
        }

        public IEnumerable<T> GetArguments<T>() => _args.OfType<T>();
    }
}