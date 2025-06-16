using FluentValidation;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Database.Cache;
using Questao5.Infrastructure.Database.Repositories;
using Questao5.Infrastructure.Sqlite;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddOptions<DistributedCacheEntryOptions>()
    .Configure(setup =>
    {
        //setup.SlidingExpiration = TimeSpan.FromMinutes(1);
    });

builder.Services.AddSingleton<IDistributedCache, SqliteCache>();

// builder.Services.AddScoped<IValidator<AddMovimentoCommand>, AddMovimentoValidator>();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IGenericRepository<ContaCorrente>, GenericRepository<ContaCorrente>>();
builder.Services.AddScoped<IGenericRepository<ContaCorrente, Movimento>, GenericRepository<ContaCorrente, Movimento>>();
builder.Services.AddScoped<IGenericRepository<Movimento, ContaCorrente>, GenericRepository<Movimento, ContaCorrente>>();
builder.Services.AddScoped<IGenericRepository<Idempotencia>, GenericRepository<Idempotencia>>();

//builder.Services.AddScoped(typeof(IGenericRepository<>), s =>
//{
//    var cnn = s.GetRequiredService<SqliteConnection>();
//    dynamic obj = Activator.CreateInstance(typeof(GenericRepository<>), cnn)!;
//    return obj;
//}
//);
// sqlite

builder.Services.AddOptions<DatabaseConfig>().Configure(setup =>
{
    setup.Name = builder.Configuration.GetValue("DatabaseName", "Data Source=database.sqlite")!;
});

builder.Services.AddScoped(provider =>
{
    var options = provider.GetRequiredService<IOptions<DatabaseConfig>>();
    return new SqliteConnection(options.Value.Name);
});

//builder.Services.AddSingleton(new DatabaseConfig { Name = builder.Configuration.GetValue("DatabaseName", "Data Source=database.sqlite") });
builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    /*c.SwaggerDoc("v1",
     * new OpenApiInfo { Title = "IdempotentAPI", Version = "v1" }
        );*/
    c.EnableAnnotations();
    //c.OperationFilter<AddHeaderParameter>();    
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

// sqlite
await app.Services.GetRequiredService<IDatabaseBootstrap>().SetupAsync();

await app.RunAsync();

// Informações úteis:
// Tipos do Sqlite - https://www.sqlite.org/datatype3.html


