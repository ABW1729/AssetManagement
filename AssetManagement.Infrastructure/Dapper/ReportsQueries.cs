using System.Data;
using AssetManagement.Application.Reports;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AssetManagement.Infrastructure.Dapper;

public class ReportsQueries : IReportsQueries
{
	private readonly string _connectionString;
	public ReportsQueries(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("Missing DefaultConnection");
	}

	public async Task<IReadOnlyList<WarrantyExpiryItem>> GetAssetsNearingWarrantyAsync(int days)
	{
		await using var conn = new SqlConnection(_connectionString);
		var sql = @"select Id as AssetId, Name, SerialNumber, WarrantyExpiryDate
			from Assets
			where WarrantyExpiryDate is not null and WarrantyExpiryDate <= DATEADD(day, @days, GETDATE())
			order by WarrantyExpiryDate";
		var rows = await conn.QueryAsync<WarrantyExpiryItem>(sql, new { days });
		return rows.ToList();
	}
}


