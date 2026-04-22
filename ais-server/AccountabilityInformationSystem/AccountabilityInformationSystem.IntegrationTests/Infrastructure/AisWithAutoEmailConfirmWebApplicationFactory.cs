using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public sealed class AisWithAutoEmailConfirmWebApplicationFactory : AisAuthWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services.PostConfigure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
            });
        });

    }
}
