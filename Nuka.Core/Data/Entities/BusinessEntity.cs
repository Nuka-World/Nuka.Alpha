using System;

namespace Nuka.Core.Data.Entities
{
    public abstract class BusinessEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public string LastUpdatedFunc { get; set; }
        public Guid CacheIndex { get; set; }
    }
}