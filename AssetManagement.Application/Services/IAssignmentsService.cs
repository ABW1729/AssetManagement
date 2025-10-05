using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.Services;

public interface IAssignmentsService
{
	Task<PagedResult<AssetAssignment>> GetHistoryAsync(int? assetId, int? employeeId, int page, int pageSize);
	Task<AssetAssignment> AssignAsync(int assetId, int employeeId, DateTime assignedDate, string? notes);
	Task<AssetAssignment?> ReturnAsync(int assignmentId, DateTime returnDate, string? notes);
}


