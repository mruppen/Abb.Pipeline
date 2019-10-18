using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abb.Pipeline
{
    public interface IPipelineBehavior
    {
        Task Handle(IPipelineExecutionContext executionContext, Func<CancellationToken, Task> next, CancellationToken token);
    }
}
