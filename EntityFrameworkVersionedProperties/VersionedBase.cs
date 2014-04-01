using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EntityFrameworkVersionedProperties {
    public abstract class VersionedBase<TVersion, TBase>
        where TVersion : VersionBase<TBase>, new() {
        /// <summary>
        /// Scalar properties
        /// </summary>
        
        [Key]
        public Int64 Id { get; protected set; }

        /// <summary>
        /// Navigation properties
        /// </summary>
        
        public virtual ICollection<TVersion> Versions { get; protected set; }

        /// <summary>
        /// Constructors
        /// </summary>
        
        protected VersionedBase() {
            Versions = new List<TVersion>();
        }

        /// <summary>
        /// Helper properties
        /// </summary>
        
        [NotMapped]
        public Boolean HasValue {
            get {
                return Versions.Any();
            }
        }

        [NotMapped]
        public TBase Value {
            get {
                if (HasValue)
                    return Versions.OrderByDescending(x => x.CreationDateTime).First().Value;
                throw new InvalidOperationException(this.GetType().Name + " has no value");
            }
            set {
                Versions.Add(new TVersion { Value = value });
            }
        }
    }
}
