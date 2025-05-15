using AuthPractic.Components;
using AuthPractic.Components.Account;
using AuthPractic.Data;
using AuthPractic.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddTransient<IEmailSender<ApplicationUser>, EmailService>();

builder.Services.AddAuthentication(options => {
	                                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	                                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                   })
       .AddJwtBearer(options => {
	                     var jwtSettings = builder.Configuration.GetSection("Jwt");
	                     var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? string.Empty);

	                     options.TokenValidationParameters = new TokenValidationParameters {
		                     ValidateIssuer = true,
		                     ValidateAudience = true,
		                     ValidIssuer = jwtSettings["Issuer"],
		                     ValidAudience = jwtSettings["Audience"],
		                     ValidateLifetime = true,
		                     ValidateIssuerSigningKey = true,
		                     IssuerSigningKey = new SymmetricSecurityKey(key)
	                     };
	                     options.Events = new JwtBearerEvents {
		                     OnMessageReceived = context => {
			                                         context.Token = context.Request.Cookies["AuthToken"];
			                                         return Task.CompletedTask;
		                                         }
	                     };
                     })
       .AddIdentityCookies();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=Data/app.db"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
       .AddEntityFrameworkStores<AppDbContext>()
       .AddSignInManager()
       .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
	                                            options.Cookie.HttpOnly = true;
	                                            options.LoginPath = "/login";
                                            });


var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	if (dbContext.Database.GetService<IDatabaseCreator>() is not RelationalDatabaseCreator databaseCreator)
		return;

	if (!databaseCreator.ExistsAsync()
	                    .GetAwaiter()
	                    .GetResult())
		await dbContext.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.UseMigrationsEndPoint();
}
else {
	app.UseExceptionHandler("/Error", true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();