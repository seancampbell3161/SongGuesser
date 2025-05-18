using API.Data;
using API.Data.Repositories;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tasks;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = "Data Source=../API/MyApp.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();