using System.Text;
using Api.Controllers;
using Api.Data;
using Api.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"];
var JwtKey = Encoding.ASCII.GetBytes(builder.Configuration["Jwt"]);

builder.Services.AddAuthentication(x => {
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=> {
  x.TokenValidationParameters = new TokenValidationParameters{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidateLifetime= true,
    ValidIssuer = "http://localhost:5224",
    ValidAudience = "http://localhost:5224",
    IssuerSigningKey = new SymmetricSecurityKey(JwtKey)
  };
});

builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connectionString));

builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<IConfiguration>();

var app = builder.Build();

var options = new DbContextOptionsBuilder<AppDBContext>()
    .UseSqlServer(connectionString)
    .Options;

var HomeController = new Home(new AppDBContext(options));
var UserController = new UsersController(new AppDBContext(options), builder.Configuration);

app.MapGet("/", HomeController.OnGetAll);
app.MapPost("/apiv1/PostNewUser", (UserDto dto) => UserController.PostNewUser(dto));

app.Run();
