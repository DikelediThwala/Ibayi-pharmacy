using IBayiLibrary.DataAccess;
using Microsoft.EntityFrameworkCore;
using ONT_PROJECT.Models;
using IBayiLibrary.Repository;

var builder = WebApplication.CreateBuilder(args);

// Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddTransient<EmailService>();

// ?? Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add custom services
builder.Services.AddTransient<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IPrescriptionRepository, PrescriptionRepository>();
builder.Services.AddTransient<IPrescriptionLineRepository, PrescriptionLineRepository>();
builder.Services.AddTransient<IUnproccessedPrescriptionRepository, UnproccessedPrescriptionRepository>();
builder.Services.AddTransient<IDoctorRepository, DoctorRepository>();

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

// ?? Enable session before authorization and endpoint mapping
app.UseSession(); // <--- this is important!

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
