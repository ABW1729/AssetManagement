using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities;

public class Employee
{
	public int Id { get; set; }

	[Required]
	[MaxLength(200)]
	public string FullName { get; set; } = string.Empty;

	[MaxLength(100)]
	public string? Department { get; set; }

	[MaxLength(200)]
	public string? Email { get; set; }

	[MaxLength(50)]
	public string? PhoneNumber { get; set; }

	[MaxLength(100)]
	public string? Designation { get; set; }

	public bool IsActive { get; set; } = true;
}


