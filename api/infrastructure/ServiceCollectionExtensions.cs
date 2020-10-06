using System;
using System.Reflection;
using JCCommon.Clients.LocationServices;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SS.Api.Helpers;
using SS.Api.Helpers.Extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.Models;
using SS.Api.services;
using BasicAuthenticationHeaderValue = SS.Api.models.BasicAuthenticationHeaderValue;

namespace SS.Api.infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMapster(this IServiceCollection services, Action<TypeAdapterConfig> options = null)
        {
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetAssembly(typeof(Startup)) ?? throw new InvalidOperationException());

            options?.Invoke(config);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }

        public static IServiceCollection AddSSServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<LocationServicesClient>(client =>
            {
                client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(
                    configuration.GetNonEmptyValue("LocationServicesClient:Username"),
                    configuration.GetNonEmptyValue("LocationServicesClient:Password"));
                client.BaseAddress = new Uri(configuration.GetNonEmptyValue("LocationServicesClient:Url").EnsureEndingForwardSlash());
            });

            services.AddHttpClient(nameof(CookieAuthenticationEvents));

            services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddScoped<ManageTypesService>();
            services.AddScoped<AuthService>();
            services.AddScoped<RoleService>();
            services.AddScoped<UserService>();
            services.AddScoped<SheriffService>();
            services.AddScoped<JustinDataUpdaterService>();

            services.AddHostedService<TimedDataUpdaterService>();

            return services;
        }
    }
}