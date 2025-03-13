using System;
using JetBrains.Annotations;

namespace DSystemUtils.Dynamic
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReflectionClassAttribute : Attribute
    {
        public string Category { get; private set; }

        public ReflectionClassAttribute() {}

        public ReflectionClassAttribute(string category)
        {
            Category = category;
        }
    }
}