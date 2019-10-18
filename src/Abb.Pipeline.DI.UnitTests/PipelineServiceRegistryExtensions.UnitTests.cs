using Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests.Definitions;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests
{
    public class PipelineServiceRegistryExtensions_AspNetCore_UnitTests
    {
        [Fact]
        public void ServiceRegistryExtensions_simple_pipeline_registration()
        {
            var provider = new Container(registry => registry.AddPipelines(typeof(PipelineServiceRegistryExtensions_AspNetCore_UnitTests)));

            var pipeline = provider.GetService<Pipeline<SimplePipelineDefinition>>();

            Assert.NotNull(pipeline);
            Assert.IsType<SimplePipelineDefinition>(pipeline);
            Assert.NotNull(((SimplePipelineDefinition)pipeline).Factory);
            Assert.IsType<PipelineObjectFactory>(((SimplePipelineDefinition)pipeline).Factory);
        }

        [Fact]
        public void ServiceRegistryExtensions_complex_pipeline_registration()
        {
            var provider = new Container(registry => registry.AddPipelines(typeof(ComplexPipelineDefinition).Assembly));

            var pipeline = provider.GetService<ComplexPipelineDefinition>();

            Assert.NotNull(pipeline);
            Assert.IsType<ComplexPipelineDefinition>(pipeline);
            Assert.IsAssignableFrom<AdditionalInheritanceLayer<ComplexPipelineDefinition>>(pipeline);
            Assert.NotNull(pipeline.Factory);
            Assert.IsType<PipelineObjectFactory>(pipeline.Factory);
        }

        [Fact]
        public async Task ServiceRegistryExtensions_shared_base_type_pipeline_registration()
        {
            var provider = new Container(registry => registry.AddPipelines(new[] { typeof(SharedBaseType).Assembly }));

            Assert.Null(provider.GetService<SharedBaseType>());

            var pipelineVersion1 = provider.GetService<SameBaseTypePipelineDefinitionVersion1>();

            Assert.NotNull(pipelineVersion1);
            Assert.IsType<SameBaseTypePipelineDefinitionVersion1>(pipelineVersion1);
            Assert.IsAssignableFrom<SharedBaseType>(pipelineVersion1);
            Assert.IsAssignableFrom<Pipeline<SharedBaseType>>(pipelineVersion1);
            Assert.NotNull(pipelineVersion1.Factory);
            Assert.IsType<PipelineObjectFactory>(pipelineVersion1.Factory);

            var result = await pipelineVersion1.Execute();

            Assert.NotEmpty(result.GetValue<string>("param1Version1"));
            Assert.Throws<ArgumentException>(() => result.GetValue<string>("param1Version2"));

            var pipelineVersion2 = provider.GetService<SameBaseTypePipelineDefinitionVersion2>();

            Assert.NotNull(pipelineVersion2);
            Assert.IsType<SameBaseTypePipelineDefinitionVersion2>(pipelineVersion2);
            Assert.IsAssignableFrom<SharedBaseType>(pipelineVersion2);
            Assert.IsAssignableFrom<Pipeline<SharedBaseType>>(pipelineVersion2);
            Assert.NotNull(pipelineVersion2.Factory);
            Assert.IsType<PipelineObjectFactory>(pipelineVersion2.Factory);

            result = await pipelineVersion2.Execute();

            Assert.Throws<ArgumentException>(() => result.GetValue<string>("param1Version1"));
            Assert.NotEmpty(result.GetValue<string>("param1Version2"));
        }
    }
}
