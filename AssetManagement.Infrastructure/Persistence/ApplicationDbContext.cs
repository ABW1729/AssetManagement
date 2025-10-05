using AssetManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}

	public DbSet<Employee> Employees => Set<Employee>();
	public DbSet<Asset> Assets => Set<Asset>();
	public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();
	public DbSet<AssetStatusLog> AssetStatusLogs => Set<AssetStatusLog>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<AssetAssignment>()
			.HasOne(a => a.Asset)
			.WithMany()
			.HasForeignKey(a => a.AssetId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<AssetAssignment>()
			.HasOne(a => a.Employee)
			.WithMany()
			.HasForeignKey(a => a.EmployeeId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<Asset>()
			.HasIndex(a => a.SerialNumber);

		builder.Entity<Employee>()
			.HasIndex(e => e.Email);
	}
}


