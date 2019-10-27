using Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests.Definitions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Abb.Pipeline.Microsoft.Extensions.DependencyInjection.UnitTests
{
    public class PipelineServiceCollectionExtensions_AspNetCore_UnitTests
    {
        [Fact]
        public void ServiceCollectionExtensions_simple_pipeline_registration()
        {
            var services = new ServiceCollection();

            services.AddPipelines(new[] { typeof(SimplePipelineDefinition).Assembly });

            var pipelineDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SimplePipelineDefinition));
            var delegateDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(PipelineObjectFactory));

            Assert.NotNull(pipelineDescriptor);
            Assert.Equal(ServiceLifetime.Transient, pipelineDescriptor.Lifetime);
            Assert.NotNull(delegateDescriptor);
            Assert.Equal(ServiceLifetime.Singleton, delegateDescriptor.Lifetime);

            var provider = services.BuildServiceProvider();

            var pipeline = provider.GetService<SimplePipelineDefinition>();

            Assert.NotNull(pipeline);
            Assert.IsType<SimplePipelineDefinition>(pipeline);
            Assert.NotNull(pipeline.Factory);
            Assert.IsType<PipelineObjectFactory>(pipeline.Factory);
        }

        [Fact]
        public void ServiceCollectionExtensions_complex_pipeline_registration()
        {
            var services = new ServiceCollection();

            services.AddPipelines(typeof(ComplexPipelineDefinition));

            var pipelineDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ComplexPipelineDefinition));
            var delegateDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(PipelineObjectFactory));

            Assert.NotNull(pipelineDescriptor);
            Assert.Equal(ServiceLifetime.Transient, pipelineDescriptor.Lifetime);
            Assert.NotNull(delegateDescriptor);
            Assert.Equal(ServiceLifetime.Singleton, delegateDescriptor.Lifetime);

            var provider = services.BuildServiceProvider();

            var pipeline = provider.GetService<ComplexPipelineDefinition>();

            Assert.NotNull(pipeline);
            Assert.IsType<ComplexPipelineDefinition>(pipeline);
            Assert.IsAssignableFrom<AdditionalInheritanceLayer<ComplexPipelineDefinition>>(pipeline);
            Assert.NotNull(pipeline.Factory);
            Assert.IsType<PipelineObjectFactory>(pipeline.Factory);
        }

        [Fact]
        public async Task ServiceCollectionExtensions_shared_base_type_pipeline_registration()
        {
            var services = new ServiceCollection();

            services.AddPipelines(new[] { typeof(SharedBaseType).Assembly });

            var pipelineDescriptors = services.Where(d => typeof(SharedBaseType).IsAssignableFrom(d.ServiceType)).ToList();
            var delegateDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(PipelineObjectFactory));

            Assert.NotEmpty(pipelineDescriptors);
            Assert.True(pipelineDescriptors.All(d => d.Lifetime == ServiceLifetime.Transient));
            Assert.NotNull(delegateDescriptor);
            Assert.Equal(ServiceLifetime.Singleton, delegateDescriptor.Lifetime);

            var provider = services.BuildServiceProvider();

            Assert.Null(provider.GetService<SharedBaseType>());

            var pipelineVersion1 = provider.GetService<SameBaseTypePipelineDefinitionVersion1>();

            Assert.NotNull(pipelineVersion1);
            Assert.IsType<SameBaseTypePipelineDefinitionVersion1>(pipelineVersion1);
            Assert.IsAssignableFrom<SharedBaseType>(pipelineVersion1);
            Assert.IsAssignableFrom<PipelineBase<SharedBaseType>>(pipelineVersion1);
            Assert.NotNull(pipelineVersion1.Factory);
            Assert.IsType<PipelineObjectFactory>(pipelineVersion1.Factory);

            var result = await pipelineVersion1.Execute();

            Assert.NotEmpty(result.GetValue<string>("param1Version1"));
            Assert.Throws<ArgumentException>(() => result.GetValue<string>("param1Version2"));

            var pipelineVersion2 = provider.GetService<SameBaseTypePipelineDefinitionVersion2>();

            Assert.NotNull(pipelineVersion2);
            Assert.IsType<SameBaseTypePipelineDefinitionVersion2>(pipelineVersion2);
            Assert.IsAssignableFrom<SharedBaseType>(pipelineVersion2);
            Assert.IsAssignableFrom<PipelineBase<SharedBaseType>>(pipelineVersion2);
            Assert.NotNull(pipelineVersion2.Factory);
            Assert.IsType<PipelineObjectFactory>(pipelineVersion2.Factory);

            result = await pipelineVersion2.Execute();

            Assert.Throws<ArgumentException>(() => result.GetValue<string>("param1Version1"));
            Assert.NotEmpty(result.GetValue<string>("param1Version2"));
        }
    }
}
