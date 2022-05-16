using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen(o => {
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PackageTrackerAPI",
        Description = "Essa é uma API para praticar C#",
        Version = "v1",
        Contact = new OpenApiContact
        {
            Name = "Henrique",
            Email = "hgomes.andrade@gmail.com",
            Url = new Uri("https://github.com/HenriqueGomesDeAndrade")
        }       
    });

    var xmlPath = Path.Combine(AppContext.BaseDirectory, "PackageTrackerAPI.xml");
    o.IncludeXmlComments(xmlPath);
});

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
