using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Domain.Entities;

public class AssetAssignment
{
	public int Id { get; set; }

	public int AssetId { get; set; }
	public int EmployeeId { get; set; }

	public DateTime AssignedDate { get; set; }
	public DateTime? ReturnedDate { get; set; }

	[MaxLength(1000)]
	public string? Notes { get; set; }

	// Navigation properties
	public Asset? Asset { get; set; }
	public Employee? Employee { get; set; }
}


