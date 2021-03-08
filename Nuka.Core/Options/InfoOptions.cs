using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuka.Core.Options
{
    /// <summary>
    /// Represents the subsection "Info" for the application settings defined
    /// for the section "Internal" on the file "appsettings.json"
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InfoOptions
    {
        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the name of the cluster service.
        /// </summary>
        /// <value>
        /// The name of the cluster service.
        /// </value>
        public string ClusterServiceName { get; set; }

        /// <summary>
        /// Gets or sets the type of the cluster service.
        /// </summary>
        /// <value>
        /// The type of the cluster service.
        /// </value>
        public string ClusterServiceType { get; set; }

        /// <summary>
        /// Gets or sets the cluster service version.
        /// </summary>
        /// <value>
        /// The SemVer of the currently deployed service, such as 1.0.2
        /// </value>
        public string ClusterServiceVersion { get; set; }
    }
}