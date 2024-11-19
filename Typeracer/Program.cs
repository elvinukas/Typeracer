// Program.cs

using Typeracer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Typeracer.Context;
using Typeracer.Services;

var builder = WebApplication.CreateBuilder(args);

// Generate an application ID (used for the leaderboard)
var applicationID = Guid.NewGuid().ToString();

// Make the application ID available via dependency injection
builder.Services.AddSingleton<string>(applicationID);

// Adding the CORS policy to allow all origins, methods and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllersWithViews();



// Configure the database provider based on the environment
if (builder.Environment.IsEnvironment("Testing"))
{
    // Use in-memory database for tests
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("IntegrationTestsDb"));
}
else
{
    // Use PostgreSQL for other environments
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Add the GraphService
builder.Services.AddScoped<IGraphService, GraphService>();

var app = builder.Build();

app.UseCors("AllowAll");

// Expose the application ID via an API endpoint
app.MapGet("/api/application-id", ([FromServices] string applicationID) => applicationID);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see 
    // https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Added the line to register the controllers (f.e. StatisticsController)
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program
{
}