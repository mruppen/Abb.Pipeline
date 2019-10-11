using Abb.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineServiceCollectionExtensions
    {
        public static IServiceCollection AddPipelines(this IServiceCollection services, params Type[] assemblyMarkerTypes)
        {
            if (assemblyMarkerTypes == null)
                throw new ArgumentNullException(nameof(assemblyMarkerTypes));

            return AddPipelines(services, assemblyMarkerTypes.Select(t => t.Assembly).Distinct().ToArray());
        }

        public static IServiceCollection AddPipelines(this IServiceCollection services, IEnumerable<Assembly> assemblies)
            => AddPipelines(services, assemblies?.ToArray() ?? new Assembly[0]);

        public static IServiceCollection AddPipelines(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            if (assemblies.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(assemblies.Length));

            services.AddSingleton<PipelineObjectFactory>(p => p.CreateInstance);

            var allTypeInfos = assemblies.SelectMany(a => a.DefinedTypes).Select(t => t.GetTypeInfo());
            foreach (var t in FindTypesThatImplement(allTypeInfos, typeof(Pipeline<>).GetTypeInfo()))
            {
                services.AddTransient(t);
            }

            foreach (var t in allTypeInfos.Where(t => t.ImplementsInterface(typeof(IPipelineBehavior).GetTypeInfo())))
            {
                services.AddTransient(typeof(IPipelineBehavior), t);
            }

            return services;
        }

        private static object CreateInstance(this IServiceProvider provider, Type t)
            => provider.GetService(t) ?? ActivatorUtilities.CreateInstance(provider, t);

        private static IEnumerable<TypeInfo> FindTypesThatImplement(IEnumerable<TypeInfo> types, TypeInfo pluginType)
            => types.Where(t => t.IsConcrete() && t.DoesInheritFrom(pluginType));

        private static bool DoesInheritFrom(this TypeInfo typeInfo, TypeInfo baseType)
        {
            if (typeInfo == typeof(object) || baseType == typeof(object))
                return false;

            if (typeInfo.BaseType.IsGenericType && typeInfo.BaseType.GetGenericTypeDefinition().GetTypeInfo() == baseType)
                return true;

            return typeInfo.BaseType.GetTypeInfo().DoesInheritFrom(baseType);
        }

        private static bool ImplementsInterface(this TypeInfo typeInfo, TypeInfo interfaceType)
            => typeInfo.ImplementedInterfaces.Any(i => i.GetTypeInfo() == interfaceType);

        private static bool IsConcrete(this TypeInfo typeInfo)
            => !typeInfo.IsAbstract && !typeInfo.IsInterface;
    }
}
