using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RoadSignCapture.Core.Companies.Commands;
using RoadSignCapture.Core.Projects.Queries;
using RoadSignCapture.Core.Services;
using RoadSignCapture.Core.Signs.Queries;
using RoadSignCapture.Core.Users.Commands;
using RoadSignCapture.Core.Users.Queries;
using RoadSignCapture.Infrastructure;
using RoadSignCapture.Infrastructure.Cache;
using RoadSignCapture.Infrastructure.Data;
using RoadSignCapture.Infrastructure.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using FluentValidation;
using RoadSignCapture.API.Validators;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//Validation
builder.Services.AddValidatorsFromAssembly(typeof(UserValidator).Assembly, includeInternalTypes: true);

//Add Serilog
builder.Host.UseSerilog((context, configuration) => 
{
    var elasticUri = context.Configuration["Elasticsearch:Uri"];
    var elasticUsername = context.Configuration["Elasticsearch:Username"];
    var elasticPassword = context.Configuration["Elasticsearch:Password"];
    
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(
            new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticUri ?? "https://es01:9200"))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"roadsigncapture-api-logs-{DateTime.UtcNow:yyyy.MM.dd}",
                NumberOfShards = 2,
                NumberOfReplicas = 1,
                ModifyConnectionSettings = conn => conn
                    .BasicAuthentication(elasticUsername, elasticPassword)
                    .ServerCertificateValidationCallback((o, certificate, chain, errors) => true),
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
                FailureCallback = e => Console.WriteLine($"[ELASTICSEARCH ERROR] {e.MessageTemplate}")
            }
        )
        .Enrich.WithProperty("Application", "RoadSignCapture.API");
});

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


builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<GetProjectHandler>();

builder.Services.AddScoped<ISignService, SignService>();
builder.Services.AddScoped<GetSignHandler>();

builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IAudiTrailService, AuditTrailService>();

builder.Services.AddInfrastructure(configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", policy =>
    {
        policy.WithOrigins("http://localhost:5050", "https://test.mjnexusystems.co.za", "http://localhost:5203")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Add Controllers ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddTransient<RoadSignCapture.API.Middleware.GlobalExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseCors("AllowWebClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<RoadSignCapture.API.Middleware.GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();