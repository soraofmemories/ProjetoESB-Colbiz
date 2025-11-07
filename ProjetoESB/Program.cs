using ProjetoESB.Core.Services;
using ProjetoESB.Infra.Conectores;
using ProjetoESB.Infra.Repositorios;
using ProjetoESB.Infra.Contexts;     // Importa a classe ESBContext do seu projeto .Infra
using ProjetoESB.Dominio.Repositorios;
using ProjetoESB.Dominio.Conectores;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // 5xx, 408 e HttpRequestException
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(7)
    }, onRetry: (outcome, timespan, retryAttempt, context) =>
    {
        // opcional: logar cada retry
        //var logger = context.Get<ILoggerFactory>()?.CreateLogger("PollyRetry") ?? null;
        // não estranhe se não passar logger aqui — pode usar policy context para logs
    });

var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)); // abre circuito após 5 falhas, espera 30

builder.Services.AddHttpClient<ConectorRest>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // recommended
    .AddPolicyHandler(retryPolicy)
    .AddPolicyHandler(circuitBreakerPolicy);

// 1. Obter a Connection String do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("ESBDatabase");

// 2. Adicionar o ESBContext ao contêiner de Injeção de Dependência
builder.Services.AddDbContext<ESBContext>(options =>
    options.UseSqlServer(connectionString)
);
// Add services to the container.


// registrar a interface (caso queira usar IConnector) (os conectores são criados sob demanda (AddTransient))
//builder.Services.AddTransient<IConector, ConectorRest>(); // se quiser resolver genérico

// se houver factory de conectores, registre a RestConnector em container específico (a factory é única na aplicação (AddSingleton))
//builder.Services.AddSingleton<IConectorFactory, ConectorFactory>();

// Configura a Injeção de Dependência para o Repositório
// AddScoped: Cria uma nova instância a cada requisição HTTP (ideal para acesso a dados)
builder.Services.AddScoped<IIntegracaoRepositorio, IntegracaoRepositorio>();
builder.Services.AddScoped<IOrquestracaoRepositorio, OrquestracaoRepositorio>();
builder.Services.AddScoped<ILogExecucaoRepositorio, LogExecucaoRepositorio>();

// Conectores e Fábrica
builder.Services.AddHttpClient();
builder.Services.AddScoped<IConector, ConectorRest>();
builder.Services.AddScoped<IConectorFactory, ConectorFactory>();

// Serviço principal de orquestração
builder.Services.AddScoped<OrquestradorService>();

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
