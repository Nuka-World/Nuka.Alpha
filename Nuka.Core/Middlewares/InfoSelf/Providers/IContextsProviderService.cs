using System.Collections.ObjectModel;

namespace Nuka.Core.Middlewares.InfoSelf.Providers
{
    /// <summary>
    /// Provides context about extra information of the host service
    /// </summary>
    public interface IContextsProviderService
    {
        /// <summary>
        /// Gets the contexts.
        /// </summary>
        /// <returns>the contexts</returns>
        ReadOnlyDictionary<string, string> GetContexts();
    }
}