using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.Services;

public interface IEmployeesService
{
	Task<PagedResult<Employee>> GetAsync(string? search, int page, int pageSize);
	Task<Employee?> GetByIdAsync(int id);
	Task<Employee> CreateAsync(Employee employee);
	Task<Employee> UpdateAsync(Employee employee);
	Task DeleteAsync(int id);
}


