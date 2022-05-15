using Microsoft.EntityFrameworkCore;
using PackageTrackerAPI.Entities;
using PackageTrackerAPI.Persistence;
using PackageTrackerAPI.Persistence.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("PackageTrackerCS");
builder.Services.AddDbContext<PackageTrackerContext>(c => c.UseSqlServer(connectionString));

builder.Services.AddScoped<IPackageRepository, PackageRepository>();

builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
