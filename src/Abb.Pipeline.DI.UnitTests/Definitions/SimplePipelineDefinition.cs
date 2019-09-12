namespace Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests.Definitions
{
    public class SimplePipelineDefinition : Pipeline<SimplePipelineDefinition>
    {
        public SimplePipelineDefinition(PipelineObjectFactory factory) : base(factory)
        {
            Factory = factory;
        }

        public PipelineObjectFactory Factory { get; }
    }
}
