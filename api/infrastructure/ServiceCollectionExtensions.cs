using System;
using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SS.Api.services;
using SS.Db.models.auth;

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
            services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);
            services.AddScoped<ManageTypesService>();
            services.AddScoped<AuthService>();
            return services;
        }

        public static MvcOptions AddDefaultAuthorizationPolicyFilter(this MvcOptions options)
        {
            // Default authorization policy enforced via a global authorization filter
            AuthorizationPolicy requireLoginPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim(Permission.Login, "TRUE")
                .Build();

            AuthorizeFilter filter = new AuthorizeFilter(requireLoginPolicy);
            options.Filters.Add(filter);
            return options;
        }
    }
}