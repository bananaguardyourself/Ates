using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TaskService.Business;
using TaskService.Data;

namespace TaskService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class TaskTrackerController : ControllerBase
	{
		private readonly ApplicationUserManager _applicationUserManager;
		private readonly TaskTrackerManager _taskTrackerManager;		

		public TaskTrackerController(
			ApplicationUserManager applicationUserManager,
			TaskTrackerManager taskTrackerManager
			)
		{
			_applicationUserManager = applicationUserManager;
			_taskTrackerManager = taskTrackerManager;
		}

		[HttpGet(Name = "GetTasksByUserIdAsync")]
		[ProducesResponseType(typeof(IEnumerable<TaskEntity>), (int)HttpStatusCode.OK)]		
		public async Task<IActionResult> GetUserTasksAsync()
		{
			var id = User.FindFirstValue("PublicId");

			var claims = ClaimsPrincipal.Current?.Identities.First().Claims.ToList();
			var publicId = claims?.FirstOrDefault(x => x.Type.Equals("PublicId", StringComparison.OrdinalIgnoreCase))?.Value;
			var userPublicId = Guid.Parse(publicId);

			var resources = await _taskTrackerManager.GetUserTasksAsync(userPublicId);

			return Ok(resources);
		}

		[HttpPost("~/create")]
		[ProducesResponseType(typeof(TaskEntity), (int)HttpStatusCode.Created)]
		public async Task<IActionResult> CreateTaskASync(string name, string description)
		{
			var task = await _taskTrackerManager.CreateTasksAsync(name, description);

			return Created("", task);
		}

		[HttpPut(Name = "CloseTaskAsync")]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		public async Task<IActionResult> CloseTaskAsync(Guid taskId)
		{
			var claims = ClaimsPrincipal.Current?.Identities.First().Claims.ToList();
			var publicId = claims?.FirstOrDefault(x => x.Type.Equals("PublicId", StringComparison.OrdinalIgnoreCase))?.Value;
			var userPublicId = Guid.Parse(publicId);

			var result = await _taskTrackerManager.CloseTasksAsync(userPublicId, taskId);
			if (!result)
				throw new Exception("Close task error");

			return NoContent();
		}

		[HttpPost("~/assign")]		
		[ProducesResponseType((int)HttpStatusCode.OK)]
		public async Task<IActionResult> AssignTasksAsync()
		{
			var claims = ClaimsPrincipal.Current?.Identities.First().Claims.ToList();
			var publicId = claims?.FirstOrDefault(x => x.Type.Equals("PublicId", StringComparison.OrdinalIgnoreCase))?.Value;
			var userPublicId = Guid.Parse(publicId);

			var user = await _applicationUserManager.GetUsersByPublicId(userPublicId);

			if (string.Compare(user.Role, Roles.Manager, true) != 0 && string.Compare(user.Role, Roles.Admin, true) != 0)
				return Unauthorized();

			var result = await _taskTrackerManager.AssignTasksAsync();
			if (!result)
				throw new Exception("Assign tasks error");

			return Ok();
		}
	}
}