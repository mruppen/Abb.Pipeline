using System.Threading;
using System.Threading.Tasks;

namespace Abb.Pipeline
{
    public interface IPipeline
    {
        Task<IPipelineExecutionContext> Execute(CancellationToken cancellationToken = default);

        Task<IPipelineExecutionContext> Execute(IPipelineExecutionContext executionContext, CancellationToken cancellationToken = default);
    }

    public interface IPipeline<T> : IPipeline
    { }
}