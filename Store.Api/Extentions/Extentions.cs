using Microsoft.AspNetCore.Mvc;
using Persistence;
using Shared.ErrorModels;
using Services;
using Domain.Contracts;
using Store.Api.Middlewares;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Persistence.Identity;
using Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace Store.Api.Extentions;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class Extentions
{
    public static IServiceCollection RegisterAllServices(this IServiceCollection services , IConfiguration configuration)
    {

        services.AddBuiltInServices();

        services.AddSwaggerServices();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        

        services.AddInfrastructureServices(configuration);

        services.AddIdentityServices();

        services.AddApplicationServices(configuration);

        services.ConfigureJwtServices(configuration);
        



       
        services.ConfigureServices();

        return services;
    }

    private static IServiceCollection AddBuiltInServices(this IServiceCollection services)
    {

        services.AddControllers();

        return services;
    }



    private static IServiceCollection ConfigureJwtServices(this IServiceCollection services , IConfiguration configuration)
    {

        var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,

                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))


            };

        });

        return services;
    }


    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {

        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<StoreIdentityDbContext>();

        return services;
    }



    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }


    private static IServiceCollection ConfigureServices(this IServiceCollection services)
    {

        services.Configure<ApiBehaviorOptions>(config =>
        {
            config.InvalidModelStateResponseFactory = (actionContext) =>
            {
                var errors = actionContext.ModelState.Where(m => m.Value.Errors.Any())
                .Select(m => new ValidationError()
                {
                    Field = m.Key,
                    Errors = m.Value.Errors.Select(errors => errors.ErrorMessage)
                });

                var response = new ValidationErrorResponse()
                {
                    Errors = errors
                };

                return new BadRequestObjectResult(response);
            };
        });

        return services;
    }

    public static async Task<WebApplication> ConfigureMiddlewares(this WebApplication app)
    {

        await app.InitializeDatabaseAsync();

        app.UseGolobalErrorHandling();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles();


        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }



    private static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        await dbInitializer.InitializeAsync();
        await dbInitializer.InitializeIdentityAsync();


        return app;
    }


    public static WebApplication UseGolobalErrorHandling(this WebApplication app)
    {
        app.UseMiddleware<GolobalErrorHandlingMiddleware>();


        return app;
    }







}
