using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class TaskTrackerController : ControllerBase
	{
		public TaskTrackerController()
		{
			
		}

		[HttpGet(Name = "DefaultGet")]
		[Authorize]
		public async Task<string> GetAsync()
		{
			return await Task.FromResult("default");
		}
	}
}