using AssetManagement.Application.Services;
using AssetManagement.Application.Shared;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Services;

public class EmployeesService : IEmployeesService
{
	private readonly ApplicationDbContext _db;

	public EmployeesService(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<PagedResult<Employee>> GetAsync(string? search, int page, int pageSize)
	{
		var query = _db.Employees.AsNoTracking();
		if (!string.IsNullOrWhiteSpace(search))
		{
			query = query.Where(e => e.FullName.Contains(search) || (e.Email ?? "").Contains(search));
		}
		var total = await query.CountAsync();
		var items = await query
			.OrderBy(e => e.FullName)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();
		return new PagedResult<Employee>
		{
			Items = items,
			TotalCount = total,
			Page = page,
			PageSize = pageSize
		};
	}

	public Task<Employee?> GetByIdAsync(int id) => _db.Employees.FindAsync(id).AsTask();

	public async Task<Employee> CreateAsync(Employee employee)
	{
		_db.Employees.Add(employee);
		await _db.SaveChangesAsync();
		return employee;
	}

	public async Task<Employee> UpdateAsync(Employee employee)
	{
		_db.Employees.Update(employee);
		await _db.SaveChangesAsync();
		return employee;
	}

	public async Task DeleteAsync(int id)
	{
		var entity = await _db.Employees.FindAsync(id);
		if (entity != null)
		{
			_db.Employees.Remove(entity);
			await _db.SaveChangesAsync();
		}
	}
}


