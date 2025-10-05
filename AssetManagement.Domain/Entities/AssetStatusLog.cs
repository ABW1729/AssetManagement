using System.ComponentModel.DataAnnotations;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities;

public class AssetStatusLog
{
	public int Id { get; set; }
	public int AssetId { get; set; }
	public AssetStatus OldStatus { get; set; }
	public AssetStatus NewStatus { get; set; }
	public DateTime ChangedAt { get; set; }
	[MaxLength(500)]
	public string? Note { get; set; }

	public Asset? Asset { get; set; }
}


