using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Abb.Pipeline
{
    public delegate object PipelineObjectFactory(Type type);

    public abstract class Pipeline<T>
    {
        private readonly static IDictionary<TypeInfo, StepDescriptor> s_analyzedTypes = new Dictionary<TypeInfo, StepDescriptor>();
        private readonly static IDictionary<Guid, ExecuteStepDelegate> s_boundDelegates = new Dictionary<Guid, ExecuteStepDelegate>();

        private readonly object _lock = new object();
        private readonly IList<StepDescriptor> _stepDescriptors = new List<StepDescriptor>();
        private readonly PipelineObjectFactory _factory;
        private readonly INamingStrategy _namingStrategy;
        private readonly IUnknownParameterBehavior _unknownParameterBehavior;

        protected Pipeline(PipelineObjectFactory factory)
            : this(factory, StrictNamingStrategy.Instance, ExceptionForUnknownParameterBehavior.Instance)
        { }

        protected Pipeline(PipelineObjectFactory factory, INamingStrategy namingStrategy)
            : this(factory, namingStrategy, ExceptionForUnknownParameterBehavior.Instance)
        { }

        protected Pipeline(PipelineObjectFactory factory, IUnknownParameterBehavior unknownParameterBehavior)
            : this(factory, StrictNamingStrategy.Instance, unknownParameterBehavior)
        { }

        protected Pipeline(PipelineObjectFactory factory, INamingStrategy namingStrategy,
                           IUnknownParameterBehavior unknownParameterBehavior)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _namingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
            _unknownParameterBehavior = unknownParameterBehavior ?? throw new ArgumentNullException(nameof(unknownParameterBehavior));
        }

        public static IPipelineExecutionContext CreateDefaultExecutionContext() => new PipelineExecutionContext();

        public async Task<IPipelineExecutionContext> Execute(IPipelineExecutionContext executionContext, CancellationToken cancellationToken = default)
        {
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var behaviors = ResolveBehaviors();

            foreach (var step in _stepDescriptors)
            {
                ExecuteStepDelegate @delegate = null;
                lock (_lock)
                {
                    if (!s_boundDelegates.TryGetValue(step.Id, out @delegate))
                    {
                        s_boundDelegates.Add(step.Id, (@delegate = step.Bind(_namingStrategy, _unknownParameterBehavior)));
                    }
                }

                await ExecuteStep(step, executionContext, cancellationToken, @delegate, behaviors);
            }

            return executionContext;
        }

        public Task<IPipelineExecutionContext> Execute(CancellationToken cancellationToken = default)
        {
            return Execute(new PipelineExecutionContext(), cancellationToken);
        }

        protected void AddStep(Type type)
        {
            var info = type.GetTypeInfo();
            if (!s_analyzedTypes.TryGetValue(info, out var stepDescriptor))
            {
                stepDescriptor = info.GetStepDescriptor();
                s_analyzedTypes[info] = stepDescriptor;
            }

            _stepDescriptors.Add(stepDescriptor);
        }

        protected void AddStep<S>() where S : class
        {
            AddStep(typeof(S));
        }

        private Task ExecuteStep(StepDescriptor step, IPipelineExecutionContext executionContext, CancellationToken cancellationToken, ExecuteStepDelegate @delegate, IPipelineBehavior[] behaviors)
        {
            var stepInstance = _factory(step.TypeInfo);
            executionContext.CurrentStep = stepInstance;

            Func<CancellationToken, Task> currentFunc = token => @delegate(stepInstance, executionContext, token);

            for (var i = behaviors.Length - 1; i >= 0; i--)
            {
                var behavior = behaviors[i];
                var previousFunc = currentFunc;
                currentFunc = token => behavior.Handle(executionContext, cancellationToken, previousFunc);
            }

            return currentFunc(cancellationToken);
        }

        private IPipelineBehavior[] ResolveBehaviors()
            => (_factory(typeof(IEnumerable<IPipelineBehavior>)) as IEnumerable<IPipelineBehavior>)?.ToArray() ?? new IPipelineBehavior[0];
    }
}
