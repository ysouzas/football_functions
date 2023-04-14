using System.IO;
using System.Text.Json;

namespace football_functions.Extensions;

public static class SerializerExtensions
{
    public static string Serialize<T>(this T objectToSerialize, JsonSerializerOptions options = null)
    {
        return JsonSerializer.Serialize(objectToSerialize, options);
    }

    public static T Deserialize<T>(this string json, JsonSerializerOptions options = null)
    {
        if (string.IsNullOrEmpty(json))
            return default;

        options ??= new() { PropertyNameCaseInsensitive = true };

        return JsonSerializer.Deserialize<T>(json, options);
    }

    public static T Deserialize<T>(this Stream stream, JsonSerializerOptions options = null)
    {
        if (stream is null)
            return default;

        options ??= new() { PropertyNameCaseInsensitive = true };

        return JsonSerializer.Deserialize<T>(stream, options);
    }
}