using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EntityFrameworkVersionedProperties {
    public abstract class VersionBase<T> {
        /// <summary>
        /// Scalar properties
        /// </summary>
        
        [Key]
        public Int64 Id { get; protected set; }
        public DateTime CreationDateTime { get; protected set; }
         
        // Value is virtual to support overriding to let deriving classes specify attributes for the property, such as [Required] to specify a non-nullable System.String
        public virtual T Value { get; internal set; }

        /// <summary>
        /// Constructors
        /// </summary>
        
        protected VersionBase() {
            CreationDateTime = DateTime.Now;
        }

        protected VersionBase(T value)
            : this() {
            Value = value;
        }
    }

    public static class Extensions {
        public static IQueryable<T> Values<TVersion, T>(this IQueryable<VersionedBase<TVersion, T>> versioneds)
            where TVersion : VersionBase<T>, new() {
                return versioneds.Select(x => x.Versions.OrderByDescending(y => y.CreationDateTime).FirstOrDefault().Value);
        }
    }
}
