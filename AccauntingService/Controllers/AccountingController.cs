using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using AccountingService.Business;
using AccountingService.Data;

namespace AccountingService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	[Authorize]
	public class AccountingController : ControllerBase
	{
		public AccountingController()
		{

		}
	}
}