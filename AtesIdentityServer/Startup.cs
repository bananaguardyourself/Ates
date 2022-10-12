using AtesIdentityServer.Business;
using AtesIdentityServer.Data;
using AtesIdentityServer.Models;
using Kafka;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AtesIdentityServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
			var connectionString = Configuration.GetConnectionString("IdentityServerConnection");

			services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

			services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

			services.AddScoped<AtesUserManager>();
			services.AddSingleton<KafkaProducer>();

			services.AddIdentityServer()
					.AddDeveloperSigningCredential()
					.AddAspNetIdentity<ApplicationUser>()					
					.AddProfileService<ProfileService>()
					.AddConfigurationStore(options =>
					{
						options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
					})
					.AddOperationalStore(options =>
					{
						options.ConfigureDbContext = b => b.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
						options.EnableTokenCleanup = true;
					});

			services.AddControllersWithViews();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			DatabaseInitializer.InitializeDatabase(app);

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}			

			//app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseRouting();

			//app.UseAuthorization();

			app.UseIdentityServer();

			app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
		}
	}
}
