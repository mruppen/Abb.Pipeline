using System;

namespace Abb.Pipeline
{
    public class ExceptionForUnknownParameterBehavior : IUnknownParameterBehavior
    {
        private const string s_message = "No value in the current execution context";

        public static IUnknownParameterBehavior Instance => new ExceptionForUnknownParameterBehavior();

        public T Handle<T>(string name)
        {
            throw new ArgumentException(s_message, name);
        }
    }
}
