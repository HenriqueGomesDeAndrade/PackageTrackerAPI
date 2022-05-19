using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PackageTrackerAPI.Emails;
using PackageTrackerAPI.Entities;
using PackageTrackerAPI.Persistence;
using PackageTrackerAPI.Persistence.Repository;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("PackageTrackerCS");
builder.Services.AddDbContext<PackageTrackerContext>(c => c.UseSqlServer(connectionString));

builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IEmailDependency, EmailDependency>();

var sendGridApiKey = builder.Configuration.GetSection("SendGridApiKey").Value;

builder.Services.AddSendGrid(o => o.ApiKey = sendGridApiKey); 

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PackageTrackerAPI",
        Description = "Essa é uma API para praticar C#. A ideia da aqui é acompanhar a entrega de um pacote, portanto cada pacote pode ter várias atualizações.",
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
//Apenas para o swagger ficar ativo mesmo fora do ambiente de desenvolvimento
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
