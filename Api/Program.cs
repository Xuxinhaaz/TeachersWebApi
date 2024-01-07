using System.Text;
using Api.Application.Mapping.Teachers;
using Api.Application.Mapping.Users;
using Api.Application.Repositories;
using Api.Application.Repositories.TeacherRepo;
using Api.Application.Repositories.User;
using Api.Application.Repositories.UserRepo;
using Api.Application.ViewModels;
using Api.Application.ViewModels.PutViewModel;
using Api.Application.ViewModels.Teachers;
using Api.Application.ViewModels.Users;
using Api.Controllers;
using Api.Data;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"];
var jwtKey = Encoding.ASCII.GetBytes(builder.Configuration["Jwt"]);
var loggerFactory = LoggerFactory.Create(builder =>
{
  builder.AddConsole();
});

builder.Services.AddDbContext<AppDBContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddAutoMapper(typeof(DomainToUserDto));
builder.Services.AddAutoMapper(typeof(DomainToUsersProfileDto));
builder.Services.AddAutoMapper(typeof(DomainToTeachersDto));
builder.Services.AddAutoMapper(typeof(DomainToTeachersProfileDto));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUsersProfileRepository, UsersProfileRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeachersProfileRepository, TeachersProfileRepository>();
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
    IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
  };
});


builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddUserSecrets<IConfiguration>();

var app = builder.Build();

var options = new DbContextOptionsBuilder<AppDBContext>()
    .UseSqlServer(connectionString)
    .Options;
var jwtServiceLogger = loggerFactory.CreateLogger<JwtService>();
var userRepository = new UserRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var usersProfileRepository = new UsersProfileRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var teacherRepository = new TeacherRepository(app.Services.GetRequiredService<IMapper>(), new AppDBContext(options));
var teachersProfileRepository = new TeachersProfileRepository(new AppDBContext(options), app.Services.GetRequiredService<IMapper>());
var jwtService = new JwtService(builder.Configuration, jwtServiceLogger);

var usersController = new UsersController(new AppDBContext(options), userRepository, usersProfileRepository, jwtService);
var teachersController = new TeachersController(new AppDBContext(options), teacherRepository, teachersProfileRepository, jwtService);

app.MapGet("/apiv1/GetUser/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization) => 
  usersController.GetUserByid(id, authorization));

app.MapGet("/apiv1/GetUsers",   
  ([FromQuery] int pageNumber, [FromHeader] string authorization) => 
  usersController.GetUsers(pageNumber, authorization));

app.MapPost("/apiv1/PostNewUser", 
  (UserViewModel viewModel) => 
  usersController.PostNewUser(viewModel));

app.MapPost("/apiv1/PostNewUsersProfile/{id}", 
  ([FromBody] UsersProfileViewModel viewModel, [FromRoute] string id, [FromHeader] string authorization) => 
  usersController.PostUsersProfile(viewModel, id, authorization));

app.MapDelete("/apiv1/DeleteUser/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization) => 
  usersController.DeleteUser(id, authorization));


app.MapPost("/apiv1/PostNewTeacher", 
  (TeacherViewModel viewModel) => 
  teachersController.PostNewTeacher(viewModel));

app.MapPost("/apiv1/PostNewTeachersProfile/{id}", 
  ([FromBody] TeachersProfileViewModel viewModel, [FromRoute] string id, [FromHeader] string authorization) => 
  teachersController.PostNewTeachersProfile(viewModel, id, authorization));

app.MapDelete("/apiv1/DeleteATeacher/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization) => 
  teachersController.DeleteATeacher(id, authorization));

app.MapGet("/apiv1/GetTeachers", 
  ([FromQuery] int pageNumber, [FromHeader] string authorization) => 
  teachersController.GetTeachers(pageNumber, authorization));

app.MapGet("/apiv1/GetTeacherByID/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization) => 
  teachersController.GetTeacherByid(id, authorization));

app.MapPut("/apiv1/PutATeacher/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization, [FromBody] PutTeachersViewModel model) =>
  teachersController.PutATeacher(id, authorization, model));

app.MapPut("/apiv1/PutTeachersProfile/{id}", 
  ([FromRoute] string id, [FromHeader] string authorization, [FromBody] TeachersProfileViewModel model) =>
  teachersController.PutTeachersProfile(id, authorization, model));


app.Run();  