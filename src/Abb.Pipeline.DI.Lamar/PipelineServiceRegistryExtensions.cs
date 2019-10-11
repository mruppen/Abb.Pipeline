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
        public static ServiceRegistry AddPipelines(this ServiceRegistry services, params Type[] assemblyMarkerTypes)
        {
            if (assemblyMarkerTypes == null)
                throw new ArgumentNullException(nameof(assemblyMarkerTypes));

            return AddPipelines(services, assemblyMarkerTypes.Select(t => t.Assembly).Distinct().ToArray());
        }

        public static ServiceRegistry AddPipelines(this ServiceRegistry services, IEnumerable<Assembly> assemblies)
            => AddPipelines(services, assemblies?.ToArray() ?? new Assembly[0]);

        public static ServiceRegistry AddPipelines(this ServiceRegistry registry, params Assembly[] assemblies)
        {
            if (registry == null)
                throw new ArgumentNullException(nameof(registry));

            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            if (assemblies.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(assemblies.Length));

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
