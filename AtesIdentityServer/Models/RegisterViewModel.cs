﻿using System.ComponentModel.DataAnnotations;

namespace AtesIdentityServer.Models
{
    public class RegisterViewModel
    {
		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }
		
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

		[Required]
		[Display(Name = "Role")]
		public string Role { get; set; }

		[Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]        
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
