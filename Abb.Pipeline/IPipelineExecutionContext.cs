namespace Abb.Pipeline
{
    public interface IPipelineExecutionContext
    {
        object CurrentStep { get; set; }

        string[] Names { get; }

        void Add<T>(string name, T value);

        T Get<T>(string name);
    }
}
