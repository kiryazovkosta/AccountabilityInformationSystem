using System;
using System.Collections.Generic;
using System.Text;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public static class Routes
{
    public static class Auth
    {
        public const string Register = "/api/identity/auth/register";
        public const string Login = "/api/identity/auth/login";
    }

    public static class Warehouses
    {
        public const string Base = "/api/warehouses";
    }
}
