namespace AssetManagement.Application.Reports;

public record WarrantyExpiryItem(int AssetId, string Name, string? SerialNumber, DateTime? WarrantyExpiryDate);

public interface IReportsQueries
{
	Task<IReadOnlyList<WarrantyExpiryItem>> GetAssetsNearingWarrantyAsync(int days);
}


