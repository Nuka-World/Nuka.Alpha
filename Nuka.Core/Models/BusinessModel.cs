using System;

namespace Nuka.Core.Models
{
    public abstract class BusinessModel : BaseModel
    {
        public virtual int Id { get; set; }
        public virtual DateTime CreatedAt { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime LastUpdatedAt { get; set; }
        public virtual string LastUpdatedBy { get; set; }
        public virtual string LastUpdatedFunc { get; set; }
        public virtual Guid CacheIndex { get; set; }
    }
}