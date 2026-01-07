using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AccountabilityInformationSystem.ConsoleSeeder;

internal static class JsonConfig
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenReading,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
}
