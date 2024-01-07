using System.Text;
using Api.Application.Mapping.Users;
using Api.Application.Repositories;
using Api.Application.Repositories.TeacherRepo;
using Api.Application.Repositories.User;
using Api.Application.ViewModels;
using Api.Application.ViewModels.PutViewModel;
using Api.Application.ViewModels.Teachers;
using Api.Application.ViewModels.Users;
using Api.Controllers;
using Api.Data;
using Api.Models.Dtos;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"];
var JwtKey = Encoding.ASCII.GetBytes(builder.Configuration["Jwt"]);

builder.Services.AddAutoMapper(typeof(DomainToUserDto));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

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
var UserRepository = new UserRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var UsersProfileRepository = new UsersProfileRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var TeacherRepository = new TeacherRepository(app.Services.GetRequiredService<IMapper>(), new AppDBContext(options));
var TeachersProfileRepository = new TeachersProfileRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var jwtService = new JwtService(builder.Configuration);

var HomeController = new Home(new AppDBContext(options));
var UserController = new UsersController(new AppDBContext(options), builder.Configuration, UserRepository, UsersProfileRepository, jwtService);
var TeacherController = new TeachersController(new AppDBContext(options), builder.Configuration, TeacherRepository, TeachersProfileRepository, jwtService);

app.MapGet("/", HomeController.OnGetAll);

app.MapGet("/apiv1/GetUser/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization) => 
  UserController.GetUserByID(ID, Authorization));

app.MapGet("/apiv1/GetUsers",   
  ([FromQuery] int pageNumber, [FromHeader] string Authorization) => 
  UserController.GetUsers(pageNumber, Authorization));

app.MapPost("/apiv1/PostNewUser", 
  (UserViewModel viewModel) => 
  UserController.PostNewUser(viewModel));

app.MapPost("/apiv1/PostNewUsersProfile/{ID}", 
  ([FromBody] UsersProfileViewModel viewModel, [FromRoute] string ID, [FromHeader] string Authorization) => 
  UserController.PostUsersProfile(viewModel, ID, Authorization));

app.MapDelete("/apiv1/DeleteUser/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization) => 
  UserController.DeleteUser(ID, Authorization));

app.MapDelete("/apiv1/DeleteMax", 
  () => 
  UserController.DeleteMax());


app.MapPost("/apiv1/PostNewTeacher", 
  (TeacherViewModel viewModel) => 
  TeacherController.PostNewTeacher(viewModel));

app.MapPost("/apiv1/PostNewTeachersProfile/{ID}", 
  ([FromBody] TeachersProfileViewModel viewModel, [FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.PostNewTeachersProfile(viewModel, ID, Authorization));

app.MapDelete("/apiv1/DeleteATeacher/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization) => 
  TeacherController.DeleteATeacher(ID, Authorization));

app.MapGet("/apiv1/GetTeachers", 
  ([FromQuery] int pageNumber, [FromHeader] string Authorization) => 
  TeacherController.GetTeachers(pageNumber, Authorization));

app.MapPut("/apiv1/PutATeacher/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization, [FromBody] PutTeachersViewModel model) =>
  TeacherController.PutATeacher(ID, Authorization, model));

app.MapPut("/apiv1/PutTeachersProfile/{ID}", 
  ([FromRoute] string ID, [FromHeader] string Authorization, [FromBody] TeachersProfileViewModel model) =>
  TeacherController.PutTeachersProfile(ID, Authorization, model));


app.Run();  