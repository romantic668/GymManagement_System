using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using GymManagement.Data;
using GymManagement.Models;
using GymManagement.Services;


var builder = WebApplication.CreateBuilder(args);

// ✅ Register MVC
builder.Services.AddControllersWithViews();

// ✅ Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Register Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
  options.Password.RequireDigit = true;
  options.Password.RequiredLength = 6;
  options.Password.RequireNonAlphanumeric = true;
  options.Password.RequireUppercase = true;
  options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ✅ Configure cookie settings (AccessDenied path)
builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Account/Login";
  options.AccessDeniedPath = "/Account/AccessDenied"; // 👈 Add this line
});

// ✅ Register Google Authentication
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
      options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
      options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
      options.CallbackPath = "/signin-google";
    });

builder.Services.AddAuthorization();

// ✅ Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UserService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Area routing - must come BEFORE default
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Dashboard}/{id?}");
// Customer route configuration
app.MapControllerRoute(
    name: "customer",
    pattern: "Customer/{action=Dashboard}/{id?}",
    defaults: new { controller = "Customer" });

// ✅ Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ Seed roles/users
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  await SeedData.InitializeAsync(services);
}

app.Run();
