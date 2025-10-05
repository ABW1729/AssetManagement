using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Web.Infrastructure;

public static class SampleDataSeeder
{
	public static async Task SeedAsync(IServiceProvider services)
	{
		using var scope = services.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		await db.Database.MigrateAsync();

		if (!await db.Employees.AnyAsync())
		{
			var emps = new[]
			{
				new Employee { FullName = "Alice Johnson", Department = "IT", Email = "alice@example.com", IsActive = true, Designation = "Engineer" },
				new Employee { FullName = "Bob Smith", Department = "HR", Email = "bob@example.com", IsActive = true, Designation = "HR Manager" },
				new Employee { FullName = "Carol Lee", Department = "Finance", Email = "carol@example.com", IsActive = true, Designation = "Analyst" }
			};
			db.Employees.AddRange(emps);
		}

		if (!await db.Assets.AnyAsync())
		{
			var assets = new[]
			{
				new Asset { Name = "Dell Latitude 7420", AssetType = "Laptop", MakeModel = "Dell 7420", SerialNumber = "DL-7420-001", PurchaseDate = DateTime.Today.AddMonths(-18), WarrantyExpiryDate = DateTime.Today.AddMonths(6), Condition = AssetCondition.Good, Status = AssetStatus.Available, IsSpare = false },
				new Asset { Name = "HP LaserJet Pro", AssetType = "Printer", MakeModel = "HP LJ P1102", SerialNumber = "HP-P1102-123", PurchaseDate = DateTime.Today.AddYears(-2), WarrantyExpiryDate = DateTime.Today.AddMonths(-1), Condition = AssetCondition.NeedsRepair, Status = AssetStatus.UnderRepair, IsSpare = false },
				new Asset { Name = "Lenovo ThinkPad T14", AssetType = "Laptop", MakeModel = "Lenovo T14", SerialNumber = "LN-T14-555", PurchaseDate = DateTime.Today.AddMonths(-3), WarrantyExpiryDate = DateTime.Today.AddYears(2), Condition = AssetCondition.New, Status = AssetStatus.Available, IsSpare = true }
			};
			db.Assets.AddRange(assets);
		}

		await db.SaveChangesAsync();

		// Seed one assignment if possible
		var firstAsset = await db.Assets.FirstOrDefaultAsync(a => a.Status == AssetStatus.Available);
		var firstEmployee = await db.Employees.FirstOrDefaultAsync();
		if (firstAsset != null && firstEmployee != null && !await db.AssetAssignments.AnyAsync())
		{
			firstAsset.Status = AssetStatus.Assigned;
			db.AssetAssignments.Add(new AssetAssignment
			{
				AssetId = firstAsset.Id,
				EmployeeId = firstEmployee.Id,
				AssignedDate = DateTime.Today.AddDays(-7),
				Notes = "Initial assignment"
			});
			await db.SaveChangesAsync();
		}
	}
}


