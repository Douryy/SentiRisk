using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SentiRisk.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SentiRiskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SentiRiskContext") ?? throw new InvalidOperationException("Connection string 'SentiRiskContext' not found.")));

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     // Cette ligne coupe la boucle infinie
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

     // Optionnel : rend le JSON plus lisible (indentation)
     options.JsonSerializerOptions.WriteIndented = true;
 });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
// ajout pour creation bdd
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<SentiRiskContext>();
   // context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}


app.Run();
