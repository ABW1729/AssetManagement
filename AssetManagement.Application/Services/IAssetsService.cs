using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Services;

public interface IAssetsService
{
	Task<PagedResult<Asset>> GetAsync(string? search, string? type, AssetStatus? status, bool? isSpare, int? assignedEmployeeId, string? sort, bool desc, int page, int pageSize);
	Task<Asset?> GetByIdAsync(int id);
	Task<Asset> CreateAsync(Asset asset);
	Task<Asset> UpdateAsync(Asset asset);
	Task DeleteAsync(int id);
}


