using System.Text;
using Api.Application.Repositories;
using Api.Application.Repositories.User;
using Api.Application.ViewModels;
using Api.Application.ViewModels.Users;
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
}).AddJwtBearer(x => {
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
var UserRepository = new UserRepository(new AppDBContext(options));
var UsersProfileRepository = new UsersProfileRepository(new AppDBContext(options));

var HomeController = new Home(new AppDBContext(options));
var UserController = new UsersController(new AppDBContext(options), builder.Configuration, UserRepository, UsersProfileRepository);
var TeacherController = new TeachersController(new AppDBContext(options), builder.Configuration);

app.MapGet("/", HomeController.OnGetAll);

app.MapGet("/apiv1/GetUser/{ID}", 
  ([FromRoute] string ID) => 
  UserController.GetUserByID(ID));
app.MapGet("/apiv1/GetUsers",   
  ([FromQuery] int pageNumber) => 
  UserController.GetUsers(pageNumber));
app.MapPost("/apiv1/PostNewUser", 
  (UserViewModel viewModel) => 
  UserController.PostNewUser(viewModel));
app.MapPost("/apiv1/PostNewUsersProfile/{ID}", 
  ([FromBody] UsersProfileViewModel viewModel, [FromRoute] string ID) => 
  UserController.PostUsersProfile(viewModel, ID));
app.MapDelete("/apiv1/DeleteUser/{ID}", 
  ([FromRoute] string ID) => 
  UserController.DeleteUser(ID));
app.MapDelete("/apiv1/DeleteMax", 
  () => 
  UserController.DeleteMax());


app.MapPost("/apiv1/PostNewTeacher", 
  (TeacherDto dto) => 
  TeacherController.PostNewTeacher(dto));
app.MapPost("/apiv1/PostNewTeachersProfile/{ID}", 
  ([FromBody] TeachersProfileDto dto, [FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.PostNewTeachersProfile(dto, ID, Authorization));
app.MapDelete("/apiv1/DeleteATeacher/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.DeleteATeacher(ID, Authorization));
app.MapGet("/apiv1/GetTeachers", 
  ([FromQuery] int pageNumber) => 
  TeacherController.GetTeachers(pageNumber));


app.Run();  