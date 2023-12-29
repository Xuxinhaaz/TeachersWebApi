using System.Text;
using Api.Controllers;
using Api.Data;
using Api.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
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
var TeacherController = new TeachersController(new AppDBContext(options), builder.Configuration);

app.MapGet("/", HomeController.OnGetAll);

app.MapGet("/apiv1/GetUser/{ID}", ([FromRoute] string ID) => UserController.GetUser(ID));
app.MapPost("/apiv1/PostNewUser", (UserDto dto) => UserController.PostNewUser(dto));
app.MapPost("/apiv1/PostNewUsersProfile/{ID}", ([FromBody] UsersProfileDto dto, [FromRoute] string ID) => UserController.PostUsersProfile(dto, ID));
app.MapDelete("/apiv1/DeleteUser/{ID}", ([FromRoute] string ID) => UserController.DeleteUser(ID));

app.MapPost("/apiv1/PostNewTeacher", 
  (TeacherDto dto) => 
  TeacherController.PostNewTeacher(dto));
app.MapPost("/apiv1/PostNewTeachersProfile/{ID}", 
  ([FromBody] TeachersProfileDto dto, [FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.PostNewTeachersProfile(dto, ID, Authorization));
app.MapDelete("/apiv1/DeleteATeacher/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.DeleteATeacher(ID, Authorization));
  

app.Run();  