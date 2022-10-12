using AtesIdentityServer.IdentityConfiguration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AtesIdentityServer.Data
{
    public static class DatabaseInitializer
    {
		public static void InitializeDatabase(IApplicationBuilder app)
		{
			using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
			serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

			var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
			context.Database.Migrate();

			if (!context.Clients.Any())
			{
				foreach (var client in Clients.Get())
				{
					context.Clients.Add(client.ToEntity());
				}
				context.SaveChanges();
			}

			if (!context.IdentityResources.Any())
			{
				foreach (var resource in Resources.GetIdentityResources())
				{
					context.IdentityResources.Add(resource.ToEntity());
				}
				context.SaveChanges();
			}

			if (!context.ApiResources.Any())
			{
				foreach (var resource in Resources.GetApiResources())
				{
					context.ApiResources.Add(resource.ToEntity());
				}
				context.SaveChanges();
			}

			if (!context.ApiScopes.Any())
			{
				foreach (var resource in Scopes.GetApiScopes())
				{
					context.ApiScopes.Add(resource.ToEntity());
				}
				context.SaveChanges();
			}

			var appContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			appContext.Database.Migrate();
			
		}
	}
}
