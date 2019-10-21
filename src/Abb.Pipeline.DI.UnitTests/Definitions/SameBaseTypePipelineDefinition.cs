using System.Threading.Tasks;

namespace Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests.Definitions
{
    public abstract class SharedBaseType : PipelineBase<SharedBaseType>
    {
        protected SharedBaseType(PipelineObjectFactory factory) : base(factory, UseDefaultValueForUnknownParameterBehavior.Instance)
        {
            Factory = factory;
        }

        public PipelineObjectFactory Factory { get; }
    }

    public class SameBaseTypePipelineDefinitionVersion1 : SharedBaseType
    {
        public SameBaseTypePipelineDefinitionVersion1(PipelineObjectFactory factory) : base(factory)
        {
            AddStep<Step1Version1>();
            AddStep<Step2>();
        }
    }

    public class SameBaseTypePipelineDefinitionVersion2 : SharedBaseType
    {
        public SameBaseTypePipelineDefinitionVersion2(PipelineObjectFactory factory) : base(factory)
        {
            AddStep<Step1Version2>();
            AddStep<Step2>();
        }
    }

    public class Step1Version1
    {
        public void Do(IPipelineExecutionContext context)
        {
            context.AddValue("param1Version1", $"Hello from {nameof(Step1Version1)}");
        }
    }

    public class Step1Version2
    {
        public Task Do(IPipelineExecutionContext context)
        {
            context.AddValue("param1Version2", $"Hello from {nameof(Step1Version2)}");
            return Task.CompletedTask;
        }
    }

    public class Step2
    {
        public string Param1Version1 { get; private set; }

        public string Param1Version2 { get; private set; }

        public void ComingTogether(string param1Version1, string param1Version2, IPipelineExecutionContext context)
        {
            Param1Version1 = param1Version1;
            Param1Version2 = param1Version2;
        }
    }
}
