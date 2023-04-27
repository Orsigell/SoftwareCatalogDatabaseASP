using Microsoft.EntityFrameworkCore;
using SoftwareCatalogDatabaseASP.Models;
using Microsoft.AspNetCore.Identity;
using SoftwareCatalogDatabaseASP.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SoftwareCatalogDatabaseASP.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<SoftwareCatalogDBContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("SoftwareCatalogDB")));
builder.Services.AddDbContext<SoftwareCatalogDatabaseASPContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("SoftwareCatalogDB")));
builder.Services.AddIdentity<SoftwareCatalogDatabaseASPUser, IdentityRole>(options =>
options.SignIn.RequireConfirmedAccount = false).
 AddEntityFrameworkStores<SoftwareCatalogDatabaseASPContext>();
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
    opt.LoginPath = new PathString("/Identity/Account/Login");
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
