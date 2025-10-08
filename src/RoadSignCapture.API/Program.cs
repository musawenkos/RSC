using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Core.Users.Queries;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// --- Database ---
builder.Services.AddDbContext<RSCDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Services ---
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRole, UserRoleService>();

builder.Services.AddScoped<UserHandler>();
builder.Services.AddScoped<GetUserHandler>();

builder.Services.AddScoped<CompanyHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", policy =>
    {
        policy.WithOrigins("http://localhost:5050", "https://test.mjnexusystems.co.za")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Add Controllers ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseCors("AllowWebClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();