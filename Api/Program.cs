using Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connectionString));

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<IConfiguration>();

var app = builder.Build();

app.MapGet("/", () => connectionString);

app.Run();
