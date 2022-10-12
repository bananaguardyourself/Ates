﻿using AtesIdentityServer.Business;
using AtesIdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AtesIdentityServer.Controllers
{
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly AtesUserManager _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IIdentityServerInteractionService _interaction;

		public AccountController(
			AtesUserManager userManager,
			SignInManager<ApplicationUser> signInManager,
			IIdentityServerInteractionService interaction)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_interaction = interaction;
		}

		[HttpGet]
		public IActionResult Login(string returnUrl)
		{
			var vm = new LoginViewModel()
			{
				ReturnUrl = returnUrl
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			// check if we are in the context of an authorization request
			var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);

				if (result.Succeeded)
				{
					if (context != null)
					{
						// we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
						return Redirect(model.ReturnUrl);
					}

					// request for a local page
					if (Url.IsLocalUrl(model.ReturnUrl))
					{
						return Redirect(model.ReturnUrl);
					}
					else if (string.IsNullOrEmpty(model.ReturnUrl))
					{
						return Redirect("~/");
					}
					else
					{
						// user might have clicked on a malicious link - should be logged
						throw new Exception("invalid return URL");
					}
				}

				ModelState.AddModelError(string.Empty, "Invalid credentials");
			}

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Logout(string logoutId)
		{
			if (User?.Identity.IsAuthenticated == true)
			{
				// delete local authentication cookie
				await _signInManager.SignOutAsync();
			}

			var logout = await _interaction.GetLogoutContextAsync(logoutId);

			return Redirect(logout.PostLogoutRedirectUri);
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			await _userManager.RegisterUser(model.Username, model.Email, model.Role, model.Password);

			return View("RegistrationSuccess");
		}
	}
}
