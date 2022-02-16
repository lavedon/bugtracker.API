using Xunit;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services;
using System.Linq;
using EntityFrameworkCoreMock;
using System.Collections.Generic;
using System;

namespace BugTracker.Tests;

public class DatabaseFixture : IDisposable
{
    public DbContextMock<AppDbContext> Db { get; private set; }
    public DbContextOptions<AppDbContext> DummyOptions { get; } = new DbContextOptionsBuilder<AppDbContext>().Options;
    public DatabaseFixture()
    {
        var initialUsers = new[]
        {
            new User { Id = 0, Username = "testAdminUser1", Password = "testPassword1", Role = "Admin"},
            new User { Id = 1, Username = "testStandardUser1", Password = "anotherTestPassword", Role = "Standard"}
        };

        var initialProjects = new[]
        {
            new Project { Id = 9, Name = "testProject1", Description = "Test Project 1", UserCreatedId = 0},
            new Project { Id = 15, Name = "testProject2", Description = "Test Project 2", UserCreatedId = 1}
        };

        var initialTickets = new[]
        {
            new Ticket { Id = 5, Name = "testTicket1", Description = "Test Ticket Description 1", ProjectId = 9, UserAssignedId = 1, UserCreatedId = 0},
            new Ticket { Id = 6, Name = "testTicket2", Description = "Test Ticket Description 2", ProjectId = 15, UserAssignedId = 0, UserCreatedId = 1}
        };

        var dbContextMock = new DbContextMock<AppDbContext>(DummyOptions);

        var usersDbSetMock = dbContextMock.CreateDbSetMock(x => x.Users, initialUsers);
        var usersDbSetProject = dbContextMock.CreateDbSetMock(x => x.Projects, initialProjects);
        var usersDbSetTicket = dbContextMock.CreateDbSetMock(x => x.Tickets, initialTickets);

        Db = dbContextMock;
    }

    public void Dispose()
    {
        Db.Object.Dispose();
    }

}

public class MainServiceTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture fixture;
    public MainServiceTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }


    [Fact(DisplayName = "Make sure the fixture is set up with 2 users")]
    public void DbSetUser_From_Fixture_Should_Have_2()
    {
        Assert.Equal(2, fixture.Db.Object.Users.Count());
    }

    [Fact(DisplayName = "Try to login with a user from our fixture")]
    public void TestUserLoginWorks()
    {

        UserService userService = new UserService();
        User? result = userService.Get(new UserLogin { Username = "testAdminUser1", Password = "testPassword1" }, fixture.Db.Object);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Make sure failed login works")]
    public void TestUserLoginFails()
    {
        UserService userService = new UserService();
        User? result = userService.Get(new UserLogin { Username = "testnUser1", Password = "wrongPassword" }, fixture.Db.Object);
        Assert.Null(result);
    }

    [Fact(DisplayName = "Make sure GetAll returns all users")]
    public void TestGetAllUsers()
    {
        UserService userService = new UserService();
        List<User> result = userService.GetAll(fixture.Db.Object);
        Assert.Equal(2, result.Count());
    }

    [Fact(DisplayName = "Get a user by name")]
    public void TestGetUserByName()
    {
        UserService userService = new UserService();
        User? result = userService.GetByName("testAdminUser1", fixture.Db.Object);
        Assert.NotNull(result);
    }

    [Fact(DisplayName = "Get a user by name that doesn't exist")]
    public void ShouldFailGetUserByName()
    {
        UserService userService = new UserService();
        User? result = userService.GetByName("ThisUserDoesNotExit", fixture.Db.Object);
        Assert.Null(result);
    }


    [Fact(DisplayName = "The test project should fail to create, because the user does not exist")]
    public void TestCreateProjectFails()
    {
        ProjectService projectService = new ProjectService();
        int? id;
        Assert.ThrowsAny<System.NullReferenceException>(() =>
         id = projectService.Create(
            new ProjectDTO { Name = "TestProject", Description = "TestDescription", UserCreated = "No Such User" }, 
            fixture.Db.Object)
            );
    }

    [Fact(DisplayName = "Delete a project by id number")]
    public void TestDeleteProject()
    {
        ProjectService projectService = new ProjectService();
        Assert.True(projectService.Delete(9, fixture.Db.Object));
        Assert.Null(projectService.GetById(9, fixture.Db.Object));
    }

    [Fact(DisplayName = "Get a project by Id number")]
    public void GetProjectById()
    {
        ProjectService projectService = new ProjectService();
        int? id = projectService.Create(new ProjectDTO { Name = "TestProject", Description = "TestDescription", UserCreated = "testAdminUser1" }, fixture.Db.Object);
        Project? result = projectService.GetById(id.Value, fixture.Db.Object);
        Assert.NotNull(result);
        Assert.IsType<Project>(result);
    }

    [Fact(DisplayName = "Get a list of all projects")]
    public void GetAllProjects()
    {
        ProjectService projectService = new ProjectService();
        List<Project> result = projectService.GetAll(fixture.Db.Object);
        Assert.Equal(2, result.Count());
    }

    [Fact(DisplayName = "Create a ticket")]
    public void CreateTicket()
    {
        TicketService ticketService = new TicketService();
        Ticket? result = ticketService.Create(new Ticket { Name = "TestTicket", Description = "TestDescription", ProjectId = 9, UserAssignedId = 1, UserCreatedId = 0 }, fixture.Db.Object);
        Assert.NotNull(result);
        Assert.IsType<Ticket>(result);
    }

    [Fact(DisplayName = "Delete a ticket by name")]
    public void DeleteTicketByName()
    {
        TicketService ticketService = new TicketService();
        Assert.True(ticketService.Delete("testTicket1", fixture.Db.Object));
    }

    [Fact(DisplayName = "Test that deleting a ticket by name can fail.")]
    public void DeleteTicketByName_ShouldFail()
    {
        TicketService ticketService = new TicketService();
        Assert.False(ticketService.Delete("This ticket does not exit", fixture.Db.Object));
    }

    [Fact(DisplayName = "Test that getting all tickets works")]
    public void GetAllTickets()
    {
        TicketService ticketService = new TicketService();
        List<Ticket> result = ticketService.GetAll(fixture.Db.Object);
        Assert.Equal(2, result.Count());
        Assert.IsType<Ticket>(result[0]);
        Assert.IsType<List<Ticket>>(result);
    }

    [Fact(DisplayName = "Test that getting all tickets by project name works")]
    public void GetAllTickets_By_ProjectName()
    {
        TicketService ticketService = new TicketService();
        List<Ticket> result = ticketService.GetByProject("testProject1", fixture.Db.Object);
        Assert.Equal(1, (int)result.Count());
        Assert.IsType<Ticket>(result[0]);
        Assert.IsType<List<Ticket>>(result);
    }

    [Fact(DisplayName = "Update a ticket")]
    public void UpdateTicket()
    {
        TicketService ticketService = new TicketService();
        bool result = ticketService.Update(new Ticket { Id = 5, Name = "TestTicket", Description = "TestDescription", ProjectId = 9, UserAssignedId = 1, UserCreatedId = 0 }, fixture.Db.Object);
        Assert.True(result);
    }

}
public class OtherServiceTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture fixture;
    public OtherServiceTests(DatabaseFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = "Have a user create a project")]
    public void TestCreateProject()
    {
        fixture.Dispose();
        fixture = new DatabaseFixture();
        // Arrange
        ProjectService projectService = new ProjectService();
        try {
        projectService.Delete(0, fixture.Db.Object);
        } catch (Exception) {
            Console.WriteLine("Tried to make sure projects were clear before test create.");
         }


        // Act
        int? createdProjectId = projectService.Create(new ProjectDTO { Name = "Another test project created by xunit", Description = "xUnit created this test project", UserCreated = "testAdminUser1" }, fixture.Db.Object);

        Assert.NotNull(createdProjectId);
        Assert.IsType<int>(createdProjectId);
        Assert.Equal(0, createdProjectId);
    }
    [Fact(DisplayName = "Delete ticket by id")]
    public void DeleteTicketById()
    {
        TicketService ticketService = new TicketService();
        Assert.True(ticketService.Delete(5, fixture.Db.Object));
    }

    [Fact(DisplayName = "Test that deleting a ticket by an id that does not exist fails")]
    public void DeleteTicketById_ShouldFail()
    {
        TicketService ticketService = new TicketService();
        Assert.False(ticketService.Delete(0, fixture.Db.Object));
    }
}