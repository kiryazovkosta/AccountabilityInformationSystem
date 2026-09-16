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

    public static class TwoFactor
    {
        public const string NewDevice = "/api/identity/auth/2fa/new-device";
        public const string Setup = "/api/identity/auth/2fa/setup";
        public const string Verify = "/api/identity/auth/2fa/verify";
    }

    public static class Warehouses
    {
        public const string Base = "/api/warehouses";

        public static string Delete(string id) => $"{Base}/{id}";
    }

    public static class ProductTypes
    {
        public const string Base = "/api/product-types";
    }

    public static class ExciseNoms
    {
        public static class ApCodes
        {
            public const string Base = "/api/excise/ap-codes";
        }

        public static class BrandNames
        {
            public const string Base = "/api/excise/brand-names";
        }

        public static class CnCodes
        {
            public const string Base = "/api/excise/cn-codes";
        }
    }

    public static class Ikunks
    {
        public const string Base = "/api/flow/ikunks";
    }

    public static class MeasuringPoints
    {
        public const string Base = "/api/flow/measuring-points";

        public static string Deactivate(string id) => $"{Base}/{id}/deactivate";
    }

    public static class WarrantyRecords
    {
        public const string Base = "/api/family/warranty-records";
    }

    public static class WarrantyBrands
    {
        public const string Base = "/api/family/warranty-brands";

        public static string Delete(string id) => $"{Base}/{id}";
    }
}
