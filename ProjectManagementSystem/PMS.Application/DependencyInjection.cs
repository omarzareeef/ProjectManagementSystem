using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace PMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        var serviceTypes = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Service"));

        foreach (var implementation in serviceTypes)
        {
            var interfaceType = implementation.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{implementation.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, implementation);
            }
        }

        return services;
    }
}
