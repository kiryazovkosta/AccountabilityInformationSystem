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
        public const string ForgotPassword = "/api/identity/auth/forgot-password";
        public const string ResetPassword = "/api/identity/auth/reset-password";
        public const string ChangePassword = "/api/identity/auth/change-password";
    }

    public static class Warehouses
    {
        public const string Base = "/api/warehouses";
    }
}
