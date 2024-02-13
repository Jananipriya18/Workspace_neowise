using BookStoreDBFirst.Models;
using BookStoreDBFirst.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<JobApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Gift")));
// Configure JWT authentication
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<JobApplicationDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IUserService, UserService>();
 builder.Services.AddScoped<CustomerRepository>();
        // builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<GiftRepository>();
        builder.Services.AddScoped<CartRepository>();  // Add this line for CartRepository
        builder.Services.AddScoped<GiftService, GiftServiceImpl>();
        builder.Services.AddScoped<CustomerService, CustomerServiceImpl>();
        builder.Services.AddScoped<CartService, CartServiceImpl>();

builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create roles if they don't exist
    if (!await roleManager.RoleExistsAsync("admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }

    if (!await roleManager.RoleExistsAsync("applicant"))
    {
        await roleManager.CreateAsync(new IdentityRole("applicant"));
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        );
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors();


// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
Console.WriteLine("bye");

app.Run();

