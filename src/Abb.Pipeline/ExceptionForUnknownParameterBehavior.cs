using System;

namespace Abb.Pipeline
{
    public class ExceptionForUnknownParameterBehavior : IUnknownParameterBehavior
    {
        public static IUnknownParameterBehavior Instance => new ExceptionForUnknownParameterBehavior();

        public T Handle<T>(string name)
        {
            throw new ArgumentException("No value in the current execution context", name);
        }
    }
}
