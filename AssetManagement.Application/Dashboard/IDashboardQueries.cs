namespace AssetManagement.Application.Dashboard;

public record AssetCounts(
	int Total,
	int Assigned,
	int Available,
	int UnderRepair,
	int Retired,
	int Spare
);

public interface IDashboardQueries
{
	Task<AssetCounts> GetCountsAsync();
	Task<IReadOnlyList<(string AssetType, int Count)>> GetCountsByTypeAsync();
}


