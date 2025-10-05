using System.Data;
using AssetManagement.Application.Dashboard;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AssetManagement.Infrastructure.Dapper;

public class DashboardQueries : IDashboardQueries
{
	private readonly string _connectionString;

	public DashboardQueries(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("Missing DefaultConnection");
	}

	public async Task<AssetCounts> GetCountsAsync()
	{
		await using var conn = new SqlConnection(_connectionString);
		var sql = @"
			select
				(SELECT count(*) FROM Assets) as Total,
				(SELECT count(*) FROM Assets where Status = 1) as Assigned,
				(SELECT count(*) FROM Assets where Status = 0) as Available,
				(SELECT count(*) FROM Assets where Status = 2) as UnderRepair,
				(SELECT count(*) FROM Assets where Status = 3) as Retired,
				(SELECT count(*) FROM Assets where IsSpare = 1) as Spare";
		var row = await conn.QuerySingleAsync(sql);
		return new AssetCounts(
			Total: (int)row.Total,
			Assigned: (int)row.Assigned,
			Available: (int)row.Available,
			UnderRepair: (int)row.UnderRepair,
			Retired: (int)row.Retired,
			Spare: (int)row.Spare
		);
	}

	public async Task<IReadOnlyList<(string AssetType, int Count)>> GetCountsByTypeAsync()
	{
		await using var conn = new SqlConnection(_connectionString);
		var sql = "select AssetType, count(*) as Cnt from Assets group by AssetType order by Cnt desc";
		var rows = await conn.QueryAsync(sql);
		return rows.Select(r => ((string)(r.AssetType ?? "Unknown"), (int)r.Cnt)).ToList();
	}
}


