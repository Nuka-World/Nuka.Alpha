using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nuka.Core.TypeFinders
{
    /// <summary>
    /// A class that finds types needed by looping assemblies in the 
    /// currently executing AppDomain. Only assemblies whose names matches
    /// certain patterns are investigated and an optional list of assemblies
    /// referenced by AssemblyNames are always investigated.
    /// </summary>
    public class AppDomainTypeFinder : ITypeFinder
    {
        /// <summary>
        /// Gets the pattern for assembly that need to be investigated.
        /// </summary>
        private string AssemblyLoadingPattern { get; } = "^Nuka";

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof(T), onlyConcreteClasses);
        }

        /// <summary>
        /// Find classes of type
        /// </summary>
        /// <param name="assignTypeFrom">Assign type from</param>
        /// <param name="onlyConcreteClasses">A value indicating whether to find only concrete classes</param>
        /// <returns>Result</returns>
        /// <returns></returns>
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        }

        /// <summary>
        /// Gets the assemblies related to the current implementation.
        /// </summary>
        /// <returns>A list of assemblies</returns>
        public IList<Assembly> GetAssemblies()
        {
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();

            AddAssembliesInAppDomain(addedAssemblyNames, assemblies);

            return assemblies;
        }

        protected virtual IEnumerable<Type> FindClassesOfType(
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

        protected virtual bool IsTypeImplementOpenGeneric(Type type, Type openGeneric)
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

        /// <summary>
        /// Check if an assembly is one of the shipped assemblies that need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The name of the assembly to check.
        /// </param>
        /// <returns>
        /// True if the assembly should be loaded into system.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName)
        {
            return Matches(assemblyFullName, AssemblyLoadingPattern);
        }

        /// <summary>
        /// Check if an assembly is one of the shipped assemblies that need to be investigated.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly name to match.
        /// </param>
        /// <param name="pattern">
        /// The regular expression pattern to match against the assembly name.
        /// </param>
        /// <returns>
        /// True if the pattern matches the assembly name.
        /// </returns>
        protected virtual bool Matches(string assemblyFullName, string pattern)
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