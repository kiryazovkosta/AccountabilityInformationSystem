using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AccountabilityInformationSystem.ConsoleSeeder;

internal class Utils
{
    public async Task<T?> SafeDeserializeAsync<T>(HttpResponseMessage response)
    {
        try
        {
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json, JsonConfig.DefaultOptions);
        }
        catch (HttpRequestException ex)
        {
            throw new HttpClientException($"Request failed: {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new HttpClientException($"Invalid JSON response: {ex.Message}");
        }
    }
}
