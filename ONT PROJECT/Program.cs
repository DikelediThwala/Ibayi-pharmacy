using IBayiLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using QuestPDF;
using QuestPDF.Infrastructure;
using IBayiLibrary.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add this line for Render deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://*:{port}");


// Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MVC controllers with views
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();

// Email service
builder.Services.AddTransient<EmailService>();

// Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //options.LoginPath = "/Account/Login";              // Redirect to login if not authenticated
        options.LoginPath = "/CustomerRegister/Login";
        options.AccessDeniedPath = "/Account/AccessDenied"; // Optional access denied page
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews();

// Set QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;
// Add custom services
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddTransient<IPrescriptionLineRepository, PrescriptionLineRepository>();
builder.Services.AddTransient<IUnproccessedPrescriptionRepository, UnproccessedPrescriptionRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();


// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session before authentication/authorization
app.UseSession();

// Authentication & Authorization middleware
app.UseAuthentication(); // MUST come before UseAuthorization
app.UseAuthorization();

// Map default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
