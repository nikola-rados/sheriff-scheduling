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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using SS.Api.infrastructure;
using SS.Api.infrastructure.authorization;
using SS.Db.models;

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
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //This should prevent resending this cookie on every request.
                options.Cookie.Path = "/api/auth";
                options.Events = new CookieAuthenticationEvents
                {
                    // After the auth cookie has been validated, this event is called.
                    // In it we see if the access token is close to expiring.  If it is
                    // then we use the refresh token to get a new access token and save them.
                    // If the refresh token does not work for some reason then we redirect to 
                    // the login screen.
                    OnValidatePrincipal = async cookieCtx =>
                    {

                        var go = cookieCtx.Properties.Items.Values.Sum(x => x.Length);
                        var accessTokenExpiration = DateTimeOffset.Parse(cookieCtx.Properties.GetTokenValue("expires_at"));
                        var timeRemaining = accessTokenExpiration.Subtract(DateTimeOffset.UtcNow);
                        var refreshThreshold = TimeSpan.Parse(Configuration.GetNonEmptyValue("TokenRefreshThreshold"));

                        if (timeRemaining > refreshThreshold)
                            return;

                        var refreshToken = cookieCtx.Properties.GetTokenValue("refresh_token");
                        var httpClientFactory = cookieCtx.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
                        var httpClient = httpClientFactory.CreateClient(nameof(CookieAuthenticationEvents));
                        var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                        {
                            Address = Configuration.GetNonEmptyValue("Keycloak:Authority") + "/protocol/openid-connect/token",
                            ClientId = Configuration.GetNonEmptyValue("Keycloak:Client"),
                            ClientSecret = Configuration.GetNonEmptyValue("Keycloak:Secret"),
                            RefreshToken = refreshToken
                        });

                        if (response.IsError)
                        {
                            cookieCtx.RejectPrincipal();
                            await cookieCtx.HttpContext.SignOutAsync(CookieAuthenticationDefaults
                                .AuthenticationScheme);
                        }
                        else 
                        {
                            var expiresInSeconds = response.ExpiresIn;
                            var updatedExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
                            cookieCtx.Properties.UpdateTokenValue("expires_at", updatedExpiresAt.ToString());
                            cookieCtx.Properties.UpdateTokenValue("access_token", response.AccessToken);
                            cookieCtx.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                            // Indicate to the cookie middleware that the cookie should be remade (since we have updated it)
                            cookieCtx.ShouldRenew = true;
                        }
                    }
                };
            }
            )
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = Configuration.GetNonEmptyValue("Keycloak:Authority");
                options.ClientId = Configuration.GetNonEmptyValue("Keycloak:Client");
                options.ClientSecret = Configuration.GetNonEmptyValue("Keycloak:Secret");
                options.RequireHttpsMetadata = true; 
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.UseTokenLifetime = true;
                options.CallbackPath = "/api/auth/signin-oidc";
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var key = Encoding.ASCII.GetBytes(Configuration.GetNonEmptyValue("Keycloak:Secret"));
                options.Authority = Configuration.GetNonEmptyValue("Keycloak:Authority");
                options.Audience = Configuration.GetNonEmptyValue("Keycloak:Audience");
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromSeconds(5)
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

            services.AddAuthorization();

            services.AddMapster();
            services.AddLazyCache();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(Configuration.GetValue<string>("CORS_DOMAIN"));
                });
            });

            services.AddDbContext<SheriffDbContext>(options =>
                {
                    options.UseNpgsql(Configuration.GetNonEmptyValue("DatabaseConnectionString"));
                    if (CurrentEnvironment.IsDevelopment())
                        options.EnableSensitiveDataLogging();
                }
            );

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSSServices(Configuration);

            services.AddControllers((opts) =>
            {
                //This fills in the claims, that AllowAnonymous wont trigger.
                if (CurrentEnvironment.IsDevelopment() && Configuration.GetNonEmptyValue("ByPassAuthAndUseImpersonatedUser").Equals("true"))
                    opts.Filters.Add<DevelopmentEnvironmentClaimsFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                options.SerializerSettings.Formatting = Formatting.Indented;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations(true);
                options.CustomSchemaIds(o => o.FullName);

                options.AddSecurityDefinition("Bearer", //Name the security scheme
                new OpenApiSecurityScheme
                {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                        Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });

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
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });
            app.UseForwardedHeaders();
            app.UpdateDatabase<Startup>();

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
                //Note this will allow access everywhere for local development. 
                if (env.IsDevelopment() && Configuration.GetNonEmptyValue("ByPassAuthAndUseImpersonatedUser").Equals("true"))
                    endpoints.MapControllers().WithMetadata(new AllowAnonymousAttribute());
                else
                    endpoints.MapControllers();
            });
        }
    }
}