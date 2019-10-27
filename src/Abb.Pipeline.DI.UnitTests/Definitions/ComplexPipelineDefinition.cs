namespace Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests.Definitions
{
    public abstract class AdditionalInheritanceLayer<T> : PipelineBase<T>
    {
        protected AdditionalInheritanceLayer(PipelineObjectFactory factory)
            : base(factory)
        {
            Factory = factory;
        }

        public PipelineObjectFactory Factory { get; }
    }


    public class ComplexPipelineDefinition : AdditionalInheritanceLayer<ComplexPipelineDefinition>
    {
        public ComplexPipelineDefinition(PipelineObjectFactory factory) : base(factory)
        {
        }
    }
}
