using Microsoft.AspNetCore.Identity;

namespace AssetManagement.Web.Infrastructure.Identity;

public static class IdentitySeeder
{
	public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
	{
		using var scope = services.CreateScope();
		var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		var adminEmail = configuration["Admin:Email"] ?? "admin@demo.local";
		var adminPassword = configuration["Admin:Password"] ?? "Admin!12345";
		var adminRole = "Admin";

		if (!await roleManager.RoleExistsAsync(adminRole))
		{
			await roleManager.CreateAsync(new IdentityRole(adminRole));
		}

		var existing = await userManager.FindByEmailAsync(adminEmail);
		if (existing == null)
		{
			var user = new IdentityUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				EmailConfirmed = true
			};
			var create = await userManager.CreateAsync(user, adminPassword);
			if (create.Succeeded)
			{
				await userManager.AddToRoleAsync(user, adminRole);
			}
		}
	}
}


