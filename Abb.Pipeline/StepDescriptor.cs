using System;
using System.Reflection;

namespace Abb.Pipeline
{
    internal struct StepDescriptor
    {
        public Guid Id { get; set; }

        public TypeInfo TypeInfo { get; set; }

        public MethodInfo Method { get; set; }

        public (string Name, TypeInfo ParameterType)[] Parameters { get; set; }
    }
}
