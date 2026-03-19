using Microsoft.EntityFrameworkCore;
using SentiRisk.Data;
using SentiRisk.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Base de données (adaptez la chaîne de connexion dans appsettings.json)
builder.Services.AddDbContext<SentiRiskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SentiRiskContext")));

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// ── Seed de la base de données ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SentiRiskContext>();
    context.Database.EnsureCreated();
    DbSeeder.Seed(context);
}

// ── Middleware de gestion des erreurs (en premier !) ──────
app.UseMiddleware<ErrorHandlingMiddleware>();

// ── Pipeline HTTP ─────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();