using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace BSE365.Repository.Extensions
{
    public static class EntityTypeConfigurationExtension
    {
        public static PrimitivePropertyConfiguration Index(this PrimitivePropertyConfiguration property, string indexName)
        {
            return property.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute(indexName)));
        }

        public static PrimitivePropertyConfiguration Index(this PrimitivePropertyConfiguration property, string indexName, int order)
        {
            return property.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute(indexName, order)));
        }

        public static PrimitivePropertyConfiguration UniqueIndex(this PrimitivePropertyConfiguration property, string indexName, int order)
        {
            return property.HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute(indexName, order) { IsUnique = true }));
        }

        public static DateTimePropertyConfiguration AsDateTime(this DateTimePropertyConfiguration property)
        {
            return property.HasColumnType("DateTime");
        }
    }
}
