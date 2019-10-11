namespace Abb.Pipeline
{
    public class UseDefaultValueForUnknownParameterBehavior : IUnknownParameterBehavior
    {
        public static IUnknownParameterBehavior Instance => new UseDefaultValueForUnknownParameterBehavior();

        public T Handle<T>(string name)
        {
            return default;
        }
    }
}
