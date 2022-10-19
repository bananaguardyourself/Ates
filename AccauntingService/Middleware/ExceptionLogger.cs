using Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AccountingService.Middleware
{
	public class ExceptionLogger
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionLogger> _logger;
		private readonly IHostEnvironment _env;
		public ExceptionLogger(RequestDelegate next, ILogger<ExceptionLogger> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}
		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex, StatusCodes.Status500InternalServerError, ex.Message);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception ex, int statusCode, string message)
		{
			_logger.LogError(ex, string.Empty);
			context.Response.StatusCode = statusCode;
			var result = new ExceptionResource(statusCode.ToString(), ex.Source ?? string.Empty, message);
			if (_env.IsDevelopment())
			{
				result.Exception = ex.ToString();
			}
			context.Response.ContentType = "application/json";
			var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
			return context.Response.WriteAsync(json);
		}
	}
}