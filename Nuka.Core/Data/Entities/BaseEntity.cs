using System;

namespace Nuka.Core.Data.Entities
{
    public abstract class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateUser { get; set; }
        public string UpdateFunction { get; set; }
    }
}