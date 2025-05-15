using System.Text.Json.Serialization;

namespace AuthPractic.Data;

public class EmailSettings {
	[JsonPropertyName(nameof(Email))] public required string Email { get; set; }

	[JsonPropertyName(nameof(Login))] public required string Login { get; set; }

	[JsonPropertyName(nameof(Password))] public required string Password { get; set; }

	[JsonPropertyName(nameof(Host))] public required string Host { get; set; }

	[JsonPropertyName(nameof(Port))] public required int Port { get; set; }

	[JsonPropertyName(nameof(UseSsl))] public bool UseSsl { get; set; }

	public bool CheckValid() {
		var isValid = !string.IsNullOrWhiteSpace(Email);

		if (string.IsNullOrWhiteSpace(Login))
			isValid = false;

		if (string.IsNullOrWhiteSpace(Password))
			isValid = false;

		if (string.IsNullOrWhiteSpace(Host))
			isValid = false;

		if (Port <= 0)
			isValid = false;

		return isValid;
	}
}