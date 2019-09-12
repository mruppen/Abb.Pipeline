namespace Abb.Pipeline
{
    public interface IUnknownParameterBehavior
    {
        T Handle<T>(string name);
    }
}
