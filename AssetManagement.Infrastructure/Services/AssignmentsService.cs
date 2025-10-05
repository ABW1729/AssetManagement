using AssetManagement.Application.Services;
using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Services;

public class AssignmentsService : IAssignmentsService
{
	private readonly ApplicationDbContext _db;

	public AssignmentsService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<AssetAssignment>> GetHistoryAsync(int? assetId, int? employeeId, int page, int pageSize)
	{
		var query = _db.AssetAssignments
			.AsNoTracking()
			.Include(a => a.Asset)
			.Include(a => a.Employee)
			.AsQueryable();

		if (assetId.HasValue) query = query.Where(x => x.AssetId == assetId);
		if (employeeId.HasValue) query = query.Where(x => x.EmployeeId == employeeId);

		var total = await query.CountAsync();
		var items = await query
			.OrderByDescending(x => x.AssignedDate)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return new PagedResult<AssetAssignment>
		{
			Items = items,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}

	public async Task<AssetAssignment> AssignAsync(int assetId, int employeeId, DateTime assignedDate, string? notes)
	{
		var asset = await _db.Assets.FindAsync(assetId) ?? throw new InvalidOperationException("Asset not found");
		if (asset.Status != AssetStatus.Available)
		{
			throw new InvalidOperationException("Asset is not available for assignment");
		}
		var employee = await _db.Employees.FindAsync(employeeId) ?? throw new InvalidOperationException("Employee not found");

		var assignment = new AssetAssignment
		{
			AssetId = assetId,
			EmployeeId = employeeId,
			AssignedDate = assignedDate,
			Notes = notes
		};
		_db.AssetAssignments.Add(assignment);

		if (asset.Status != AssetStatus.Assigned)
		{
			_db.AssetStatusLogs.Add(new AssetStatusLog
			{
				AssetId = asset.Id,
				OldStatus = asset.Status,
				NewStatus = AssetStatus.Assigned,
				ChangedAt = DateTime.UtcNow,
				Note = "Assigned"
			});
			asset.Status = AssetStatus.Assigned;
		}
		await _db.SaveChangesAsync();

		return assignment;
	}

	public async Task<AssetAssignment?> ReturnAsync(int assignmentId, DateTime returnDate, string? notes)
	{
		var assignment = await _db.AssetAssignments.FindAsync(assignmentId);
		if (assignment == null) return null;
		if (assignment.ReturnedDate.HasValue) return assignment;

		assignment.ReturnedDate = returnDate;
		assignment.Notes = notes ?? assignment.Notes;

		var asset = await _db.Assets.FindAsync(assignment.AssetId);
		if (asset != null && asset.Status != AssetStatus.Available)
		{
			_db.AssetStatusLogs.Add(new AssetStatusLog
			{
				AssetId = asset.Id,
				OldStatus = asset.Status,
				NewStatus = AssetStatus.Available,
				ChangedAt = DateTime.UtcNow,
				Note = "Returned"
			});
			asset.Status = AssetStatus.Available;
		}
		await _db.SaveChangesAsync();
		return assignment;
	}
}


