using System.ComponentModel.DataAnnotations;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities;

public class Asset
{
	public int Id { get; set; }

	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(100)]
	public string? AssetType { get; set; }

	[MaxLength(200)]
	public string? MakeModel { get; set; }

	[MaxLength(200)]
	public string? SerialNumber { get; set; }

	public DateTime? PurchaseDate { get; set; }

	public DateTime? WarrantyExpiryDate { get; set; }

	public AssetCondition Condition { get; set; } = AssetCondition.Good;

	public AssetStatus Status { get; set; } = AssetStatus.Available;

	public bool IsSpare { get; set; }

	[MaxLength(2000)]
	public string? Specifications { get; set; }
}


