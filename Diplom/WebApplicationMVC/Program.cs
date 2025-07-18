using Microsoft.EntityFrameworkCore;
using WebApplicationMVC.Auth;
using WebApplicationMVC.Models.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlite("Data Source=app.db");
});

builder.Services.AddScoped<UserManager>();

builder.Services.AddAuthentication()
	.AddCookie(AuthSettings.AuthCookieName, options =>
	{
		options.LoginPath = "/auth/login";
		options.AccessDeniedPath = "/auth/forbidden";
		options.Cookie.Name = AuthSettings.AuthCookieName;
 	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("admin", 
		policy => policy.RequireClaim(AppRole.Admin.ToString(), "true"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();