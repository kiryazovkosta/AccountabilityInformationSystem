using AccountabilityInformationSystem.ConsoleSeeder.DTOs;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AccountabilityInformationSystem.ConsoleSeeder
{
    internal class Program
    {
        public static async Task Main()
        {
            using HttpClient client = new();
            client.BaseAddress = new(GlobalConstants.baseUrl);
            LoginRequest login = new() { Email = "kosta@example.com", Password = "Test@123" };

            HttpResponseMessage response = await client.PostAsJsonAsync<LoginRequest>(GlobalConstants.authEndpoint, login);

            var utils = new Utils();
            try
            {
                LoginResponse? loginResponse = await utils.SafeDeserializeAsync<LoginResponse>(response);

                if (loginResponse is null)
                {
                    Console.WriteLine("Response body was empty or could not be deserialized.");
                    return;
                }

                CancellationToken cancellation = new();

                var seeder = new Seeder(loginResponse.AccessToken);
                var insertedRecords = await seeder.SeedProductTypesAsync(cancellation);
                Console.WriteLine($"New Product Types: {insertedRecords}");
            }
            catch (HttpClientException ex)
            {
                Console.WriteLine($"Request/deserialization failed: {ex.Message}");
            }
        }
    }
}
