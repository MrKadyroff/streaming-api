using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StreamApi
{
    public class NullableSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null) return;

            var nullableProperties = schema.Properties
                .Where(x => x.Value.Nullable == true && x.Value.Default == null)
                .ToList();

            foreach (var property in nullableProperties)
            {
                property.Value.Nullable = true;
            }

            // Убираем required для nullable полей
            if (schema.Required != null)
            {
                var nullablePropertyNames = nullableProperties.Select(x => x.Key);
                schema.Required = schema.Required.Except(nullablePropertyNames).ToHashSet();
            }
        }
    }
}
