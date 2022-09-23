using Microsoft.AspNetCore.Mvc;

namespace TaskService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DefaultController : ControllerBase
	{
		public DefaultController()
		{
			
		}

		[HttpGet(Name = "DefaultGet")]
		public async Task<string> GetAsync()
		{
			return await Task.FromResult("default");
		}
	}
}