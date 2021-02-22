using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nuka.Core.TypeFinders
{
    /// <summary>
    /// Classes implementing this interface provide information about types 
    /// to various services
    /// </summary>
    public interface ITypeFinder
    {
        IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true);
        
        IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true);
        
        IEnumerable<Assembly> GetAssemblies();
    }
}