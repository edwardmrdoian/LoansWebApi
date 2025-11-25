using FluentValidation;
using FluentValidation.AspNetCore;
using Loans.Api.Middlewares;
using Loans.Api.Swagger;
using Loans.Application.Interfaces;
using Loans.Application.Mapping;
using Loans.Application.Services;
using Loans.Application.Settings;
using Loans.Application.Validators;
using Loans.Contracts;
using Loans.Infrastructure;
using Loans.Infrastructure.Repositories;
using Loans.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


internal class Program
{
    private static void Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Debug("Init Program");
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            //Logging
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // DbContext
            builder.Services.AddDbContext<RepositoryContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

            // Repositories
            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ILoanRepository, LoanRepository>();

            //Services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ILoanService, LoanService>();

            //Logger wrapper
            builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

            //AutoMapper
            builder.Services.AddAutoMapper(m =>
            {
                m.AddProfile(new MappingProfile());
                m.LicenseKey = configuration.GetSection("AutoMapper:LicenseKey").Value;
            });



            //Controllers and JSON options
            builder.Services.AddControllers(
                options =>
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                    options.Filters.Add(new ConsumesAttribute("application/json"));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddNewtonsoftJson();

            //Fluent Validation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>()
                .AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            //JWT Settings 
            builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are not configured properly.");

            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            //Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

            //Authorization
            builder.Services.AddAuthorizationBuilder()
                    .AddPolicy("RequireAccountant", policy => policy.RequireRole("Accountant"))
                    .AddPolicy("RequireUser", policy => policy.RequireRole("User"));


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Loans Api",
                    Version = "v1",
                    Description = "API documentation with JWT Bearer token support"
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token.\n\nExample: Bearer abcdef12345"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.OperationFilter<SwaggerErrorResponsesOperationFilter>();
            });

            var app = builder.Build();

            //Global Exception Handling Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            // Migrate DB automatically 
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
                db.Database.Migrate();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Program stopped unexpectedly");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}