using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Abb.Pipeline
{
    internal delegate Task ExecuteStepDelegate(object stepInstance,
        IPipelineExecutionContext context,
        INamingStrategy namingStrategy,
        IUnknownParameterBehavior unknownParameterBehavior,
        CancellationToken cancellationToken);

    internal static class StepDescriptorBinder
    {
        internal static ExecuteStepDelegate CreateDelegate(this StepDescriptor step)
        {
            var arguments = new List<Expression>();
            var executionContextParameter = Expression.Parameter(typeof(IPipelineExecutionContext));
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));
            var namingStrategyParameter = Expression.Parameter(typeof(INamingStrategy));
            var unknownParameterBehaviorParameter = Expression.Parameter(typeof(IUnknownParameterBehavior));
            var instanceParameter = Expression.Parameter(typeof(object));

            for (var i = 0; i < step.Parameters.Length; i++)
            {
                var (parameterName, parameterType) = step.Parameters[i];
                if (parameterType == typeof(IPipelineExecutionContext))
                {
                    arguments.Add(executionContextParameter);
                }
                else if (parameterType == typeof(CancellationToken))
                {
                    arguments.Add(cancellationTokenParameter);
                }
                else
                {
                    var allNames = Expression.Property(executionContextParameter, "Names");
                    var getParameterName = Expression.Call(namingStrategyParameter, "FindMatch", Array.Empty<Type>(), Expression.Constant(parameterName), allNames);
                    var body = Expression.Call(executionContextParameter, "Get", new[] { parameterType }, getParameterName);

                    var handleUnkownParameter = Expression.Call(unknownParameterBehaviorParameter, "Handle", new Type[] { parameterType }, Expression.Constant(parameterName));
                    var @catch = Expression.Catch(typeof(ArgumentException), handleUnkownParameter);
                    var @try = Expression.TryCatch(body, @catch);
                    arguments.Add(@try);
                }
            }

            var methodCall = Expression.Call(Expression.Convert(instanceParameter, step.TypeInfo), step.Method, arguments);

            if (!typeof(Task).IsAssignableFrom(step.Method.ReturnType))
            {
                var result = Expression.Constant(Task.CompletedTask);
                var wrapper = Expression.Block(typeof(Task), methodCall, result);
                return Expression.Lambda<ExecuteStepDelegate>(wrapper, instanceParameter, executionContextParameter, namingStrategyParameter,
                    unknownParameterBehaviorParameter, cancellationTokenParameter).Compile();
            }

            return Expression.Lambda<ExecuteStepDelegate>(methodCall, instanceParameter, executionContextParameter, namingStrategyParameter,
                unknownParameterBehaviorParameter, cancellationTokenParameter).Compile();
        }
    }
}
