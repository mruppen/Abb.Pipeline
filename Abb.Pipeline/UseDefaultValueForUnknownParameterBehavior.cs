namespace Abb.Pipeline
{
    public class UseDefaultValueForUnknownParameterBehavior : IUnknownParameterBehavior
    {
        public static IUnknownParameterBehavior Instance { get; } = new UseDefaultValueForUnknownParameterBehavior();

        public T Handle<T>(string name)
        {
            return default(T);
        }
    }
}
