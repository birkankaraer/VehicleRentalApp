using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VehicleRentalApp.Data;
using VehicleRentalApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný Baðlantýsý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik doðrulama
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidateAudience = true,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,
ValidIssuer = "VehicleRentalApp",
ValidAudience = "VehicleRentalApp",
IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperLongSecureSecretKey1234567890!"))
};

// Token'ýn Header'da olduðundan emin olun
options.Events = new JwtBearerEvents
{
OnMessageReceived = context =>
{
    // Cookie üzerinden token'ý alýp header'a eklemek
    var token = context.Request.Cookies["auth_token"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Token = token;
    }
    return Task.CompletedTask;
}
};
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<TokenService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

