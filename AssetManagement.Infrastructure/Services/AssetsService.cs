using AssetManagement.Application.Services;
using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Services;

public class AssetsService : IAssetsService
{
	private readonly ApplicationDbContext _db;

	public AssetsService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<Asset>> GetAsync(string? search, string? type, AssetStatus? status, bool? isSpare, int? assignedEmployeeId, string? sort, bool desc, int page, int pageSize)
	{
		var query = _db.Assets.AsNoTracking().AsQueryable();
		if (!string.IsNullOrWhiteSpace(search))
		{
			query = query.Where(a => a.Name.Contains(search) || (a.SerialNumber ?? "").Contains(search));
		}
		if (!string.IsNullOrWhiteSpace(type))
		{
			query = query.Where(a => a.AssetType == type);
		}
		if (status.HasValue)
		{
			query = query.Where(a => a.Status == status);
		}
		if (isSpare.HasValue)
		{
			query = query.Where(a => a.IsSpare == isSpare);
		}

		if (assignedEmployeeId.HasValue)
		{
			var empId = assignedEmployeeId.Value;
			query = query.Where(a => _db.AssetAssignments.Any(x => x.AssetId == a.Id && x.EmployeeId == empId && x.ReturnedDate == null));
		}

		query = (sort, desc) switch
		{
			("name", false) => query.OrderBy(a => a.Name),
			("name", true) => query.OrderByDescending(a => a.Name),
			("type", false) => query.OrderBy(a => a.AssetType),
			("type", true) => query.OrderByDescending(a => a.AssetType),
			("status", false) => query.OrderBy(a => a.Status),
			("status", true) => query.OrderByDescending(a => a.Status),
			("serial", false) => query.OrderBy(a => a.SerialNumber),
			("serial", true) => query.OrderByDescending(a => a.SerialNumber),
			_ => query.OrderBy(a => a.Name)
		};
		var total = await query.CountAsync();
		var items = await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		return new PagedResult<Asset>
		{
			Items = items,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}

	public Task<Asset?> GetByIdAsync(int id) => _db.Assets.FindAsync(id).AsTask();

	public async Task<Asset> CreateAsync(Asset asset)
	{
		_db.Assets.Add(asset);
		await _db.SaveChangesAsync();
		return asset;
	}

	public async Task<Asset> UpdateAsync(Asset asset)
	{
		var existing = await _db.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == asset.Id)
			?? throw new InvalidOperationException("Asset not found");
		if (existing.Status != asset.Status)
		{
			_db.AssetStatusLogs.Add(new AssetStatusLog
			{
				AssetId = asset.Id,
				OldStatus = existing.Status,
				NewStatus = asset.Status,
				ChangedAt = DateTime.UtcNow,
				Note = "Manual update"
			});
		}
		_db.Assets.Update(asset);
		await _db.SaveChangesAsync();
		return asset;
	}

	public async Task DeleteAsync(int id)
	{
		var entity = await _db.Assets.FindAsync(id);
		if (entity != null)
		{
			_db.Assets.Remove(entity);
			await _db.SaveChangesAsync();
		}
	}
}


