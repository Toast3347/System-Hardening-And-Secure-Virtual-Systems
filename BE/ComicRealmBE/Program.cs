using ComicRealmBE.Data;
using ComicRealmBE.Data.Configuration;
using Microsoft.EntityFrameworkCore;

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

// ========== SERVICES ==========
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

app.UseHttpsRedirection();

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
