using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMS.Application.Interfaces.AuthService;
using PMS.Application.Interfaces.EmailService;
using PMS.Application.Interfaces.HttpService;
using PMS.Application.Interfaces.JwtService;
using PMS.Application.Interfaces.Persistence;
using PMS.Domain.Entities;
using PMS.Infrastructure.Data;
using PMS.Infrastructure.Services.AuthService;
using PMS.Infrastructure.Services.EmailService;
using PMS.Infrastructure.Services.HttpService;
using PMS.Infrastructure.Services.JwtService;
using PMS.Infrastructure.Services.Persistence;
using PMS.Infrastructure.Settings;

namespace PMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IHttpService, HttpService>();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<URLs>(configuration.GetSection("URLs"));

        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));


        services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));

        services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }
}
