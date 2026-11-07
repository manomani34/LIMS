using System.Text.Json;
using System.Text.Json.Serialization;

namespace LIMS.Api.Configuration;

public static class JsonConfiguration
{
    public static IMvcBuilder AddLimsJsonOptions(
        this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase;

            options.JsonSerializerOptions.DictionaryKeyPolicy =
                JsonNamingPolicy.CamelCase;

            options.JsonSerializerOptions.DefaultIgnoreCondition =
                JsonIgnoreCondition.WhenWritingNull;

            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

            options.JsonSerializerOptions.NumberHandling =
                JsonNumberHandling.Strict;
        });

        return builder;
    }
}
