using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sombra.IntegrationTests;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json");
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SombraDb>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            
            
            var connectionString = configuration.GetConnectionString("TestConnection");

            
            
            services.AddDbContext<SombraDb>(options =>
            {
                options.UseSqlServer(connectionString)
                    .UseSeeding((context, _) => DbSeeder.CreatePosts(context));
            });
        });
        
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        
        var db = scope.ServiceProvider.GetRequiredService<SombraDb>();
        db.Database.Migrate();
        
        return host;
    }
}