using Abb.Pipeline;
using Lamar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineServiceRegistryExtensions
    {
        public static ServiceRegistry AddPipelines(this ServiceRegistry registry)
            => AddPipelines(registry, AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).Distinct());

        public static ServiceRegistry AddPipelines(this ServiceRegistry registry, IEnumerable<Assembly> assemblies)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            registry.AddSingleton<PipelineObjectFactory>(p => p.CreateInstance);

            registry.Scan(scan =>
            {
                foreach (var assembly in assemblies)
                    scan.Assembly(assembly);

                scan.ConnectImplementationsToTypesClosing(typeof(IPipeline<>));
                scan.AddAllTypesOf<IPipelineBehavior>().NameBy(type => type.Name.ToLowerInvariant());
            });

            return registry;
        }

        private static object CreateInstance(this IServiceProvider provider, Type t)
            => provider.GetService(t) ?? ActivatorUtilities.CreateInstance(provider, t);
    }
}
