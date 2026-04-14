using ComicRealmBE.Data;
using ComicRealmBE.Data.Configuration;
using ComicRealmBE.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURATION & SECRETS ==========
// Load user secrets in development environment
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// ========== DATABASE SETUP ==========
// Get connection string from User Secrets (dev) or environment variables (prod)
var connectionString = DatabaseConfig.GetConnectionString(builder.Configuration);

// Register DbContext with PostgreSQL
builder.Services.AddDbContext<ComicRealmContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsqlOptions => npgsqlOptions.CommandTimeout(30) // 30 second timeout for long queries
    )
);

// ========== JWT ===============
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "JWT configuration is missing. Please set Jwt:Key, Jwt:Issuer, and Jwt:Audience in appsettings or environment variables.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ========== SERVICES ==========
builder.Services.AddScoped<AuthService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ========== CORS CONFIGURATION ==========
// Configure CORS for frontend communication (adjust origins as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vite default ports
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply CORS before other middleware
app.UseCors("AllowFrontend");

var isRunningInContainer = string.Equals(
    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
    "true",
    StringComparison.OrdinalIgnoreCase);

if (!isRunningInContainer)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========== DATABASE INITIALIZATION ==========
// OPTIONAL: Uncomment to auto-migrate on startup (development only)
// if (app.Environment.IsDevelopment())
// {
//     using (var scope = app.Services.CreateScope())
//     {
//         var db = scope.ServiceProvider.GetRequiredService<ComicRealmContext>();
//         db.Database.Migrate(); // Apply pending migrations
//     }
// }

app.Run();