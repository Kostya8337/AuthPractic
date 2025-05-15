using System.ComponentModel.DataAnnotations;

namespace AuthPractic.Data;

public class LoginModel {
	[Required]
	[EmailAddress]
	[Display(Name = "Email")]
	public string Email { get; set; } = string.Empty;

	[Required]
	[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
	[DataType(DataType.Password)]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;

	[Display(Name = "Remember me?")] public bool RememberMe { get; set; }
}