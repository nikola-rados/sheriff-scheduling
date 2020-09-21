using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using SS.Api.Helpers;
using SS.Api.Helpers.Middleware;
using SS.Api.Models.DB;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SS.Api.infrastructure;

namespace SS.Api
{
    public class Startup
    {
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IClaimsTransformation, ClaimsTransformer>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.Cookie.HttpOnly = true;
                //Important to be None, otherwise a redirect loop will occur.
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            }
            )
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = Configuration.GetValue<string>("Keycloak:Authority");
                options.ClientId = Configuration.GetValue<string>("Keycloak:Client");
                options.ClientSecret = Configuration.GetValue<string>("Keycloak:Secret");
                options.RequireHttpsMetadata = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("Keycloak:Secret"));
                options.Authority = Configuration.GetValue<string>("Keycloak:Authority");
                options.Audience = Configuration.GetValue<string>("Keycloak:Audience");
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
                if (key.Length > 0) options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(key);
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        throw context.Exception;
                    },
                    OnForbidden = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
                {
                    options.AddPolicy("log in user",
                        policy => policy.RequireClaim("CAN_LOGIN", "TRUE"));
                }
            );


            services.AddMapster();
            services.AddLazyCache();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("CORS_DOMAIN"));
                });
            });

            var enableSensitiveDataLogging = CurrentEnvironment.IsDevelopment();
            services.AddDbContext<SheriffDbContext>(options => options.UseNpgsql(Configuration.GetNonEmptyValue("ConnectionStrings.DB")).EnableSensitiveDataLogging(enableSensitiveDataLogging));
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSSServices(Configuration);

            services.AddControllers(options => options.AddDefaultAuthorizationPolicyFilter()).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations(true);
                options.CustomSchemaIds(o => o.FullName);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/swagger/{documentname}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "SS.Api");
                options.RoutePrefix = "api";
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}