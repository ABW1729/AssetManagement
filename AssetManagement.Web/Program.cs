using AssetManagement.Web.Components;
using AssetManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Web.Infrastructure.Identity;
using AssetManagement.Application.Services;
using AssetManagement.Infrastructure.Services;
using AssetManagement.Application.Dashboard;
using AssetManagement.Infrastructure.Dapper;
using AssetManagement.Application.Reports;
using AssetManagement.Web.Infrastructure;
using AssetManagement.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Database and Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=AssetManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// Application services
builder.Services.AddScoped<IEmployeesService, EmployeesService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IAssignmentsService, AssignmentsService>();
builder.Services.AddScoped<IDashboardQueries, DashboardQueries>();
builder.Services.AddScoped<IReportsQueries, ReportsQueries>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapRazorPages();

// Redirect any attempt to access register to login
app.MapGet("/Identity/Account/Register", (HttpContext ctx) => Results.Redirect("/Identity/Account/Login")).AllowAnonymous();

// Seed admin user on startup
await IdentitySeeder.SeedAdminAsync(app.Services, app.Configuration);
await SampleDataSeeder.SeedAsync(app.Services);

// CSV export endpoint for warranty expiry
app.MapGet("/export/warranty", async (HttpContext http, IReportsQueries reports, int days) =>
{
	var items = await reports.GetAssetsNearingWarrantyAsync(days == 0 ? 30 : days);
	var rows = new List<string> { "Asset,Serial,WarrantyExpiry" };
	foreach (var i in items)
	{
		var name = Escape(i.Name);
		var serial = Escape(i.SerialNumber);
		var date = i.WarrantyExpiryDate?.ToString("yyyy-MM-dd") ?? string.Empty;
		rows.Add($"{name},{serial},{date}");
	}
	var csv = string.Join("\n", rows);
	var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
	http.Response.Headers.ContentDisposition = $"attachment; filename=warranty_expiry_{DateTime.Today:yyyyMMdd}.csv";
	return Results.File(bytes, "text/csv");

	static string Escape(string? s)
	{
		if (string.IsNullOrEmpty(s)) return string.Empty;
		if (s.Contains(',') || s.Contains('"'))
		{
			return "\"" + s.Replace("\"", "\"\"") + "\"";
		}
		return s;
	}
}).RequireAuthorization();

// CSV export for filtered assets
app.MapGet("/export/assets", async (
	HttpContext http,
	IAssetsService assets,
	string? search,
	string? type,
	AssetStatus? status,
	bool? isSpare,
	int? assignedEmployeeId,
	string? sort,
	bool desc,
	int page = 1,
	int pageSize = 1000) =>
{
	var result = await assets.GetAsync(search, type, status, isSpare, assignedEmployeeId, sort, desc, page, pageSize);
	var rows = new List<string> { "Name,Type,Serial,Condition,Status,Spare" };
	foreach (var a in result.Items)
	{
		rows.Add($"{Escape(a.Name)},{Escape(a.AssetType)},{Escape(a.SerialNumber)},{a.Condition},{a.Status},{(a.IsSpare ? "Yes" : "No")}");
	}
	var csv = string.Join("\n", rows);
	var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
	http.Response.Headers.ContentDisposition = $"attachment; filename=assets_{DateTime.Today:yyyyMMdd}.csv";
	return Results.File(bytes, "text/csv");

	static string Escape(string? s)
	{
		if (string.IsNullOrEmpty(s)) return string.Empty;
		if (s.Contains(',') || s.Contains('"'))
		{
			return "\"" + s.Replace("\"", "\"\"") + "\"";
		}
		return s;
	}
}).RequireAuthorization();

app.Run();
