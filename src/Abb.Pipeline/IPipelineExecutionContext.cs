using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Abb.Pipeline.UnitTests")]

namespace Abb.Pipeline
{
    public interface IPipelineExecutionContext
    {
        internal object CurrentStep { get; set; }

        string[] ParameterNames { get; }

        void AddValue<T>(string name, T value);

        T GetValue<T>(string name);
    }
}
