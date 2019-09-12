using System;
using System.Linq;
using System.Reflection;

namespace Abb.Pipeline
{
    internal static class TypeAnalyzer
    {
        public static StepDescriptor GetStepDescriptor(this TypeInfo type)
        {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            if (methods?.Length == 0)
                throw new Exception();

            var validMethods = FindMethodWithParameter(methods, typeof(IPipelineExecutionContext));
            if (validMethods?.Length != 1)
                throw new Exception();

            var method = validMethods.First();

            return new StepDescriptor
            {
                Id = Guid.NewGuid(),
                TypeInfo = type,
                Method = method,
                Parameters = method.GetParameters().Select(p => (p.Name, p.ParameterType.GetTypeInfo())).ToArray()
            };
        }

        private static MethodInfo[] FindMethodWithParameter(MethodInfo[] methods, params Type[] parameterTypes)
        {
            return methods.Where(mi =>
            {
                var parameters = mi.GetParameters().Select(p => p.ParameterType);
                return parameterTypes.All(p => parameters.Contains(p));

            }).ToArray();
        }
    }
}
