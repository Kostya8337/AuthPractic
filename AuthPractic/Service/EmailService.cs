using System.Net;
using System.Net.Mail;
using AuthPractic.Data;
using Microsoft.AspNetCore.Identity;

namespace AuthPractic.Service;

public class EmailService : IEmailSender<ApplicationUser> {

	private readonly EmailSettings? _emailConfig;


	public EmailService(IConfiguration configuration) {
		var config = configuration.GetSection("EmailSettings")
		                          .Get<EmailSettings>();
		if (config == null)
			return;

		_emailConfig = config;
		if (!_emailConfig.CheckValid())
			_emailConfig = null;
	}
	public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) {
		if (_emailConfig == null)
			return Task.CompletedTask;

		var smtpClient = new SmtpClient(_emailConfig.Host) {
			Port = _emailConfig.Port,
			Credentials = new NetworkCredential(_emailConfig.Login, _emailConfig.Password),
			EnableSsl = _emailConfig.UseSsl
		};

		return smtpClient.SendMailAsync(new MailMessage(_emailConfig.Email, email, "Register", confirmationLink) {
			IsBodyHtml = true
		});
	}

	public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) {
		return Task.CompletedTask;
	}

	public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) {
		return Task.CompletedTask;
	}
}