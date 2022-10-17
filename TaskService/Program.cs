using Contracts.Settings;
using DataAccess;
using DataAccess.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Kafka;
using Microsoft.OpenApi.Models;
using TaskService.Business;
using TaskService.Data;
using TaskService.Kafka;

var builder = WebApplication.CreateBuilder(args);
var identityAuthorityUrl = "https://localhost:5001";

// Add services to the container.
builder.Services.AddSingleton<IHostedService, UserConsumerHostedService>();
builder.Services.AddSingleton<ApplicationUserRepository>();
builder.Services.AddSingleton<TaskRepository>();
builder.Services.AddSingleton<ApplicationUserManager>();
builder.Services.AddSingleton<TaskTrackerManager>();
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
	c =>
	{
		c.SwaggerDoc("v1",
			new OpenApiInfo
			{
				Title = "Task Tracker",
				Version = "v1",
			});
		c.CustomSchemaIds(x => x.FullName);
		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = SecuritySchemeType.ApiKey,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = ParameterLocation.Header,
			Description = "JWT Authorization header using the Bearer scheme."
		});
		c.AddSecurityRequirement(new OpenApiSecurityRequirement()
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					},
					Scheme = "oauth2",
					Name = "Bearer",
					In = ParameterLocation.Header,

				},
				new List<string>()
			}
		});
	});
builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
				.AddIdentityServerAuthentication(options =>
				{
					// name of the API resource
					options.ApiName = "tasktracker";
					// base-address of your identityserver
					options.Authority = identityAuthorityUrl;

				});



builder.Services.Configure<DbConnectionOptions>(builder.Configuration.GetSection(DbConnectionOptions.DbConnection));
builder.Services.AddScoped<IDefaultRepository, PostgresDefaultRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
