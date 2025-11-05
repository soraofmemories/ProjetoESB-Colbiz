using Microsoft.EntityFrameworkCore;
using ProjetoESB.Dominio.Repositorios;
using ProjetoESB.Infra.Contexts;  // Importa a classe ESBContext do seu projeto .Infra
using ProjetoESB.Infra.Repositorios; 

var builder = WebApplication.CreateBuilder(args);

// 1. Obter a Connection String do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ESBDatabase");

// 2. Adicionar o ESBContext ao contêiner de Injeção de Dependência
builder.Services.AddDbContext<ESBContext>(options =>
    options.UseSqlServer(connectionString)
);
// Add services to the container.

// Configura a Injeção de Dependência para o Repositório
// AddScoped: Cria uma nova instância a cada requisição HTTP (ideal para acesso a dados)
builder.Services.AddScoped<IIntegracaoRepositorio, IntegracaoRepositorio>();

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
