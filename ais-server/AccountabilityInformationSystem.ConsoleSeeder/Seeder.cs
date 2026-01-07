using AccountabilityInformationSystem.ConsoleSeeder.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http; // Add this using directive for HttpClient
using System.Net.Http.Json;
using System.Text;

namespace AccountabilityInformationSystem.ConsoleSeeder
{
    // Remove the constructor parameter 'IHttpClientFactory factory' to match the provided type signature
    internal class Seeder(string accessToken)
    {
        private static readonly HttpClient _httpClient = new();

        internal async Task<int> SeedProductTypesAsync(CancellationToken cancellation)
        {
            _httpClient.BaseAddress = new Uri(GlobalConstants.baseUrl);
            _httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {accessToken}");

            List<string> types = [
                "Суровина",
                "Стоков продукт",
                "Полуфабрикати и компоненти",
                "Адитиви",
                "Смазочни масла за съоръжения",
                "Смазочни масла за влагане в производството",
                "Добавки и присадки",
                "Ремонт",
                "Енергоресурси",
                "Химикали"
            ];

            int insertedRecords = 0;
            HttpResponseMessage response = await _httpClient.GetAsync(GlobalConstants.productTypes, cancellation);
            if (response.IsSuccessStatusCode) 
            {
                var utils = new Utils();
                PagedResponse<object>? result = await utils.SafeDeserializeAsync<PagedResponse<object>>(response);
                if (result is not null && result.Items.Length == 0)
                {
                    foreach (var type in types) 
                    {
                        var request = new CreateProductTypeRequest() { Name = type, FullName = type };
                        await _httpClient.PostAsJsonAsync<CreateProductTypeRequest>(GlobalConstants.productTypes, request, cancellation);
                        insertedRecords++;
                    }
                }
            }

            
            return insertedRecords; 
        }
    }
}
