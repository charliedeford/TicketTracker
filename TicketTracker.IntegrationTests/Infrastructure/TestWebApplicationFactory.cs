using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketTracker.Api.Data;

namespace TicketTracker.IntegrationTests.Infrastructure;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TicketTrackerContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TicketTrackerContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase");
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<TicketTrackerContext>();

                db.Database.EnsureCreated();

                // Add any test data seeding here if needed
                // InitializeDbForTests(db);
            }
        });
    }

    // private void InitializeDbForTests(TicketTrackerContext db)
    // {
    //     // Add test data seeding logic here
    // }
}