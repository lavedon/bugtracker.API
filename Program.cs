using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore.Design;
using System.Security.Claims;
using BugTracker.Services;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IProjectService, ProjectService>();
builder.Services.AddSingleton<ITicketService, TicketService>();
builder.Services.AddControllers().AddJsonOptions(x =>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "JWT Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

#if DEBUG
app.UseCors(policy =>
    policy.WithOrigins("https://localhost:7045")
        .AllowAnyMethod()
		.AllowAnyHeader()
        .AllowCredentials());


#else
app.UseCors(policy =>
    policy.WithOrigins("https://lukebug.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

#endif 

// User endpoints
app.MapPost("/api/login", 
[AllowAnonymous]
(UserLogin user, IUserService service, AppDbContext db) => Login(user, service, db));

app.MapGet("/api/getallusers",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
 (IUserService service, AppDbContext db) => GetAllUsers(service, db));
// @TODO make a registration endpoint with a different DTO

// Project end points
app.MapGet("/api/getallprojects",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
(IProjectService service, AppDbContext db) => GetAllProjects(service, db));

app.MapGet("/api/getprojectbyid/{id}",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
 (int id, IProjectService service, AppDbContext db) => GetProjectById(id, service, db))
    .Produces<Project>();

app.MapGet("/api/getprojectbyname/{name}",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
 (string name, IProjectService service, AppDbContext db) => GetProjectByName(name, service, db))
    .Produces<Project>();

app.MapDelete("/api/deleteproject/{id}",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
 (int id, IProjectService service, AppDbContext db) => DeleteProject(id, service, db));

app.MapPut("/api/updateproject/{id}", 
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
(Project project, IProjectService service, AppDbContext db) => UpdateProject(project, service, db));

app.MapPost("/api/createproject", 
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
(ProjectDTO project, IProjectService service,[FromServices] AppDbContext db) => CreateProject(project, service, db))
    .Accepts<ProjectDTO>("application/json")
    .Produces<Project>(statusCode: 200, contentType: "application/json");

// Ticket end points
app.MapPost("/api/createticket", 
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
(TicketDTO ticket, IProjectService projectService, IUserService userService, ITicketService ticketService, AppDbContext db) 
    => CreateTicket(ticket, projectService, userService, ticketService, db))
    .Accepts<TicketDTO>("application/json")
    .Produces<Ticket>(statusCode: 200, contentType: "application/json");

app.MapGet("/api/getalltickets",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Standard, Admin")]
(ITicketService ticketService, IUserService userService, IProjectService projectService, AppDbContext db) => GetAllTickets(ticketService, db));

IResult CreateTicket(TicketDTO ticket, IProjectService projectService, IUserService userService, ITicketService ticketService, AppDbContext db)
{
    var project = projectService.GetByName(ticket.Project, db);
    if (project == null)
    {
        return Results.BadRequest($"Project {ticket.Project} does not exist");
    }
    var userCreated = userService.GetByName(ticket.UserCreated, db);
    if (userCreated == null)
    {
        return Results.BadRequest($"User {ticket.UserCreated} does not exist");
    }
    var userAssigned = userService.GetByName(ticket.UserAssigned!, db);
    if (userAssigned == null)
    {
        return Results.BadRequest($"User {ticket.UserAssigned!} does not exist");
    }
    var newTicket = new Ticket
    {
        Name = ticket.Name,
        Description = ticket.Description,
        Project = project,
        UserCreated = userCreated,
        UserAssigned = userAssigned
    };
    db.Tickets.Add(newTicket);
    db.SaveChanges();
    
    return Results.Ok(newTicket);
}

IResult UpdateProject(Project newProject, IProjectService service, AppDbContext db)
{
    Project updatedProject = service.Update(newProject, db);

    if (updatedProject is null) Results.NotFound("Movie not found");

    return Results.Ok(updatedProject);
}

IResult DeleteProject(int id, IProjectService service, AppDbContext db)
{
    var result = service.Delete(id, db);

    if (!result) Results.BadRequest("Could not delete project");

    return Results.Ok(result);
}

IResult CreateProject(ProjectDTO project, IProjectService service, AppDbContext db)
{
    int id = service.Create(project, db);
    return Results.Created($"/CreateProject/{id}", project);
}

IResult GetProjectById(int id, IProjectService service, AppDbContext db)
{
    Project project = service.GetById(id, db);
    return Results.Ok(project);
}

IResult GetProjectByName(string name, IProjectService service, AppDbContext db)
{
    Project project = service.GetByName(name, db);
    return Results.Ok(project);
}

IResult GetAllProjects(IProjectService service, AppDbContext db)
{
    List<Project> projects = service.GetAll(db);
    return Results.Ok(projects);
}

IResult GetAllUsers(IUserService service, AppDbContext db)
{
    var users = service.GetAll(db);
    return Results.Ok(users);
}

IResult GetAllTickets(ITicketService service, AppDbContext db)
{
    var tickets = service.GetAll(db);
    return Results.Ok(tickets);
}

IResult Login(UserLogin user, IUserService service, AppDbContext db)
{
    if (!string.IsNullOrEmpty(user.Username) &&
        !string.IsNullOrEmpty(user.Password))
    {
        var loggedInUser = service.Get(user, db);
        if (loggedInUser is null)
        { 
            return Results.NotFound("User not found");
        } 
        else {

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loggedInUser.Username),
            new Claim(ClaimTypes.Role, loggedInUser.Role)    
        };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(tokenString);
        }
    }
    return Results.BadRequest("Invalid user credentials");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server v1");
   c.RoutePrefix = String.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();

public partial class Program { }