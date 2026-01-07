using System;
using System.Collections.Generic;
using System.Text;

namespace AccountabilityInformationSystem.ConsoleSeeder
{
    internal static class GlobalConstants
    {
        internal const string baseUrl = "https://localhost:4001";

        internal const string authEndpoint = "api/identity/auth/login";

        internal const string productTypes = $"{baseUrl}/api/ProductTypes";
    }
}
