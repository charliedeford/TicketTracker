using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Headers;

namespace TicketTracker.IntegrationTests.Infrastructure;

public abstract class IntegrationTest : IClassFixture<TestWebApplicationFactory<Program>>
{
    protected readonly TestWebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;

    protected IntegrationTest(TestWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    protected void AuthenticateClient(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}