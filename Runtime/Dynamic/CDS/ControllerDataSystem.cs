using System;
using System.Collections.Generic;
using System.Linq;
using DSystem;
using JetBrains.Annotations;

namespace DSystemUtils.Dynamic.CDS
{
    [AutoRegistry, UsedImplicitly]
    public class ControllerDataSystem : DSystemBase, IUpdatable
    {
        private readonly Dictionary<Type, List<object>> _controllersData = new ();

        public T GetController<T>(bool uniqueInstance = false) where T : class
        {
            return GetController(typeof(T), uniqueInstance) as T;
        }

        public object GetController(Type type, bool uniqueInstance = false)
        {
            var controllers = _controllersData.GetValueOrDefault(type);
            if (controllers == null)
            {
                controllers = new List<object>();
                _controllersData.Add(type, controllers);
            }
            
            if (uniqueInstance)
                return CreateInstance(type, controllers);
            
            var controller = controllers.FirstOrDefault();
            controller ??= CreateInstance(type, controllers);
            return controller;
        }

        public void AddController(object controller)
        {
            var type = controller.GetType();
            var controllers = _controllersData.GetValueOrDefault(type);
            if (controllers == null)
            {
                controllers = new List<object>();
                _controllersData.Add(type, controllers);
            }
            if (controllers.Contains(controller)) return;
            controllers.Add(controller);
            if (controller is IEventDisposable eventDisposable)
                eventDisposable.PreDispose += OnPreDispose;
        }

        public void RemoveController(object controller)
        {
            var type = controller.GetType();
            var controllers = _controllersData.GetValueOrDefault(type);
            controllers?.Remove(controller);
            if (controller is IEventDisposable eventDisposable)
                eventDisposable.PreDispose -= OnPreDispose;
        }

        private object CreateInstance(Type type, List<object> controllers)
        {
            var controller = Activator.CreateInstance(type);
            controllers.Add(controller);
            if (controller is IEventDisposable eventDisposable)
                eventDisposable.PreDispose += OnPreDispose;

            return controller;
        }

        private void OnPreDispose(object controller, DynamicArguments args)
        {
            RemoveController(controller);
        }

        void IUpdatable.Update()
        {
            foreach (var pair in _controllersData)
            {
                foreach (var controller in pair.Value)
                {
                    if (controller is IUpdatable updatable)
                    {
                        updatable.Update();
                    }
                }
            }
        }
    }
}