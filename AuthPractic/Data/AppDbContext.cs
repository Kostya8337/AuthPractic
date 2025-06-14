﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthPractic.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser> {
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}