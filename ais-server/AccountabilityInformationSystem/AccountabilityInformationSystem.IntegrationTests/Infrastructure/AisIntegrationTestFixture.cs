using System;
using System.Collections.Generic;
using System.Text;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public abstract class AisIntegrationTestFixture(AisWebApplicationFactory factory) 
    : IClassFixture<AisWebApplicationFactory>
{
    public HttpClient CreateClient() => factory.CreateClient();
}
