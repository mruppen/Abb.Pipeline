using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Abb.Pipeline.UnitTests
{
    public class Pipeline_UnitTests
    {
        [Fact]
        public async Task Pipeline_default_constructor_basic_operation_successful()
        {
            var testPipeline = new DefaultConstructorBasicOperationSuccessfulPipeline();

            var context = await testPipeline.Execute();

            Assert.Equal(DefaultConstructorBasicOperationSuccessfulPipeline.Step3.Parameter.Value, context.Get<int?>(DefaultConstructorBasicOperationSuccessfulPipeline.Step3.Parameter.Key));
        }

        [Fact]
        public async Task Pipeline_default_constructor_unknown_parameter_throws_exception()
        {
            var testPipeline = new DefaultConstructorUnknownParameterThrowsExceptionPipeline();

            await Assert.ThrowsAsync<ArgumentException>(() => testPipeline.Execute());
        }

        [Fact]
        public async Task Pipeline_default_constructor_unmatched_parameter_name_throws_exception()
        {
            var testPipeline = new DefaultConstructorUnmatchedNameThrowsExceptionPipeline();

            await Assert.ThrowsAsync<ArgumentException>(() => testPipeline.Execute());
        }

        [Fact]
        public async Task Pipeline_use_default_values_behavior_passes_default_value_to_method()
        {
            var testPipeline = new UseDefaultValuesForUnknownParametersPipeline();

            var result = await testPipeline.Execute();

            Assert.Equal(UseDefaultValuesForUnknownParametersPipeline.Step1.Parameter.Value,
                result.Get<string>(UseDefaultValuesForUnknownParametersPipeline.Step1.Parameter.Key));
        }


        private class DefaultConstructorBasicOperationSuccessfulPipeline : Pipeline<DefaultConstructorBasicOperationSuccessfulPipeline>
        {
            public static object GetInstanceOfStep(Type type)
            {
                if (type == typeof(Step1))
                    return new Step1();

                if (type == typeof(Step2))
                    return new Step2();

                if (type == typeof(Step3))
                    return new Step3();

                if (type == typeof(IEnumerable<IPipelineBehavior>))
                    return new IPipelineBehavior[] { new Behavior() };

                return null;
            }

            public DefaultConstructorBasicOperationSuccessfulPipeline() : base(GetInstanceOfStep)
            {
                AddStep<Step1>();
                AddStep<Step2>();
                AddStep<Step3>();
            }

            public class Step1
            {
                public readonly static KeyValuePair<string, string> Parameter = new KeyValuePair<string, string>("param1", "Hello world");

                public void Execute(IPipelineExecutionContext context)
                {
                    context.Add(Parameter.Key, Parameter.Value);
                }
            }

            public class Step2
            {
                public readonly static KeyValuePair<string, DateTimeOffset> Parameter = new KeyValuePair<string, DateTimeOffset>("param2", new DateTimeOffset(2019, 10, 10, 12, 13, 14, TimeSpan.Zero));

                public Task Execute(string param1, IPipelineExecutionContext context)
                {
                    context.Add(Parameter.Key, Parameter.Value);
                    return Task.CompletedTask;
                }
            }

            public class Step3
            {
                public readonly static KeyValuePair<string, int?> Parameter = new KeyValuePair<string, int?>("param3", null);

                public Task DoMyStuff(DateTimeOffset param2, IPipelineExecutionContext context)
                {
                    context.Add(Parameter.Key, Parameter.Value);
                    return Task.CompletedTask;
                }
            }

            public class Behavior : IPipelineBehavior
            {
                public async Task Handle(IPipelineExecutionContext executionContext, CancellationToken cancellationToken, Func<CancellationToken, Task> next)
                {
                    await next(cancellationToken);

                    if (executionContext.CurrentStep is Step1)
                    {
                        Assert.Equal(Step1.Parameter.Value, executionContext.Get<string>(Step1.Parameter.Key));
                    }
                    else if (executionContext.CurrentStep is Step2)
                    {
                        Assert.Equal(Step2.Parameter.Value, executionContext.Get<DateTimeOffset>(Step2.Parameter.Key));
                    }
                    else if (executionContext.CurrentStep is Step3)
                    {
                        Assert.Equal(Step3.Parameter.Value, executionContext.Get<int?>(Step3.Parameter.Key));
                    }
                }
            }
        }

        private class DefaultConstructorUnknownParameterThrowsExceptionPipeline : Pipeline<DefaultConstructorUnknownParameterThrowsExceptionPipeline>
        {
            public static object GetInstanceOfStep(Type type)
            {
                if (type == typeof(Step1))
                    return new Step1();

                if (type == typeof(Step2))
                    return new Step2();


                return null;
            }

            public DefaultConstructorUnknownParameterThrowsExceptionPipeline()
                : base(GetInstanceOfStep)
            {
                AddStep<Step1>();
                AddStep<Step2>();
            }

            public class Step1
            {
                public readonly static KeyValuePair<string, string> Parameter = new KeyValuePair<string, string>("param1", "Hello world");

                public void Execute(IPipelineExecutionContext context)
                {
                    Assert.NotNull(context);
                    context.Add(Parameter.Key, Parameter.Value);
                }
            }

            public class Step2
            {
                public readonly static KeyValuePair<string, DateTimeOffset> Parameter = new KeyValuePair<string, DateTimeOffset>("param2", new DateTimeOffset(2019, 10, 10, 12, 13, 14, TimeSpan.Zero));

                public Task Execute(string unknownParam1, int unknownParam2, IPipelineExecutionContext context)
                {
                    return Task.CompletedTask;
                }
            }

            public class Behavior : IPipelineBehavior
            {
                public async Task Handle(IPipelineExecutionContext executionContext, CancellationToken cancellationToken, Func<CancellationToken, Task> next)
                {
                    await next(cancellationToken);

                    if (executionContext.CurrentStep is Step1)
                    {
                        Assert.Equal(Step1.Parameter.Value, executionContext.Get<string>(Step1.Parameter.Key));
                    }
                }
            }
        }

        private class DefaultConstructorUnmatchedNameThrowsExceptionPipeline : Pipeline<DefaultConstructorUnknownParameterThrowsExceptionPipeline>
        {
            public static object GetInstanceOfStep(Type type)
            {
                if (type == typeof(Step1))
                    return new Step1();

                if (type == typeof(Step2))
                    return new Step2();


                return null;
            }

            public DefaultConstructorUnmatchedNameThrowsExceptionPipeline()
                : base(GetInstanceOfStep)
            {
                AddStep<Step1>();
                AddStep<Step2>();
            }

            public class Step1
            {
                public readonly static KeyValuePair<string, string> Parameter = new KeyValuePair<string, string>("param1", "Hello world");

                public void Execute(IPipelineExecutionContext context)
                {
                    Assert.NotNull(context);
                    context.Add(Parameter.Key, Parameter.Value);
                }
            }

            public class Step2
            {
                public readonly static KeyValuePair<string, DateTimeOffset> Parameter = new KeyValuePair<string, DateTimeOffset>("param2", new DateTimeOffset(2019, 10, 10, 12, 13, 14, TimeSpan.Zero));

                public Task Execute(string PARAM1, IPipelineExecutionContext context)
                {
                    Assert.NotNull(context);
                    context.Add(Parameter.Key, Parameter.Value);
                    return Task.CompletedTask;
                }
            }

            public class Behavior : IPipelineBehavior
            {
                public Task Handle(IPipelineExecutionContext executionContext, CancellationToken token, Func<CancellationToken, Task> next)
                {
                    throw new NotImplementedException();
                }
            }
        }

        private class UseDefaultValuesForUnknownParametersPipeline : Pipeline<DefaultConstructorUnknownParameterThrowsExceptionPipeline>
        {
            public static object GetInstanceOfStep(Type type)
            {
                if (type == typeof(Step1))
                    return new Step1();

                if (type == typeof(Step2))
                    return new Step2();


                return null;
            }

            public UseDefaultValuesForUnknownParametersPipeline()
                : base(GetInstanceOfStep, UseDefaultValueForUnknownParameterBehavior.Instance)
            {
                AddStep<Step1>();
                AddStep<Step2>();
            }

            public class Step1
            {
                public readonly static KeyValuePair<string, string> Parameter = new KeyValuePair<string, string>("param1", "Hello world");

                public void Execute(IPipelineExecutionContext context)
                {
                    Assert.NotNull(context);
                    context.Add(Parameter.Key, Parameter.Value);
                }
            }

            public class Step2
            {
                public readonly static KeyValuePair<string, DateTimeOffset> Parameter = new KeyValuePair<string, DateTimeOffset>("param2", new DateTimeOffset(2019, 10, 10, 12, 13, 14, TimeSpan.Zero));

                public Task Execute(string unknownParam1, int unknownParam2, IPipelineExecutionContext context)
                {
                    Assert.NotNull(context);
                    Assert.Null(unknownParam1);
                    Assert.Equal(default(int), unknownParam2);
                    context.Add(Parameter.Key, Parameter.Value);
                    return Task.CompletedTask;
                }
            }
        }
    }
}
