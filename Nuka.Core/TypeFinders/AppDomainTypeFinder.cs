using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nuka.Core.TypeFinders
{
    public class AppDomainTypeFinder : ITypeFinder
    {
        private string AssemblyLoadingPattern { get; } = "^Nuka";

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        public IEnumerable<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            AddAssembliesInAppDomain(addedAssemblyNames, assemblies);

            return assemblies;
        }

        private IEnumerable<Type> FindClassesOfType(
            Type assignTypeFrom, 
            IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true)
        {
            var result = new List<Type>();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsInterface)
                        continue;

                    if (assignTypeFrom.IsAssignableFrom(type)
                        || assignTypeFrom.IsGenericTypeDefinition && IsTypeImplementOpenGeneric(type, assignTypeFrom))
                    {
                        if (onlyConcreteClasses)
                        {
                            if (type.IsClass && !type.IsAbstract)
                            {
                                result.Add(type);
                            }
                        }
                        else
                        {
                            result.Add(type);
                        }
                    }
                }
            }

            return result;
        }

        private bool IsTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                return type.FindInterfaces((_, _) => true, null)
                    .Where(implementedInterface => implementedInterface.IsGenericType)
                    .Any(implementedInterface =>
                        genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
            }
            catch
            {
                return false;
            }
        }

        private bool Matches(string assemblyFullName)
        {
            return Matches(assemblyFullName, AssemblyLoadingPattern);
        }

        private bool Matches(string assemblyFullName, string pattern)
        {
            return Regex.IsMatch(assemblyFullName, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private void AddAssembliesInAppDomain(List<string> addedAssemblyNames, List<Assembly> assemblies)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!Matches(assembly.FullName))
                    continue;

                if (addedAssemblyNames.Contains(assembly.FullName))
                    continue;

                assemblies.Add(assembly);
                addedAssemblyNames.Add(assembly.FullName);
            }
        }
    }
}