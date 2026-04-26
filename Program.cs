using ConsumersVoiceSystemPrototype.Data;
using ConsumersVoiceSystemPrototype.Models;
using ConsumersVoiceSystemPrototype.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using CloudinaryDotNet;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var pgConn = Environment.GetEnvironmentVariable("DATABASE_URL");
var usePostgres = pgConn != null;

string connectionString;
if (pgConn != null)
{
    // Convert postgresql://user:pass@host:port/db → Npgsql format
    var uri = new Uri(pgConn);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')}" +
                       $";Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("No connection string found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (usePostgres)
        options.UseNpgsql(connectionString);
    else
        options.UseSqlite(connectionString);
});

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<AppNotificationService>();
builder.Services.AddScoped<ComplaintWorkflowService>();
builder.Services.AddScoped<ComplaintAttachmentStorage>();
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});
builder.Services.AddHttpContextAccessor();

// Cloudinary
builder.Services.AddSingleton<Cloudinary>(_ =>
{
    var c = builder.Configuration.GetSection("Cloudinary");
    return new Cloudinary(new Account(c["CloudName"], c["ApiKey"], c["ApiSecret"]))  { Api = { Secure = true } };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await DbInitializer.SeedAsync(app.Services);

app.Run();
