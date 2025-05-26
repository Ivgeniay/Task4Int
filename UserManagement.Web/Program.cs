using Microsoft.AspNetCore.Authentication.JwtBearer;
using UserManagement.Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using UserManagement.Persistence.Repositories;
using UserManagement.Persistence.Interfaces;
using UserManagement.Web.Middleware;
using UserManagement.Web.Services;
using UserManagement.Persistence;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using UserManagement.Web.Extensions;

namespace UserManagement.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString(WebConstants.Configuration.CONNECTION_STR)));
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Headers.ContainsKey(WebConstants.Auth.AUTHORIZATION_HEADER))
                            {
                                var authHeader = context.Request.Headers[WebConstants.Auth.AUTHORIZATION_HEADER].ToString();
                                if (authHeader.StartsWith(WebConstants.Auth.BEARER_PREFIX, StringComparison.OrdinalIgnoreCase))
                                {
                                    context.Token = authHeader[WebConstants.Auth.BEARER_PREFIX.Length..];
                                }
                                return Task.CompletedTask;
                            }

                            var token = context.Request.Cookies[WebConstants.Auth.JWT_COOKIE_NAME];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("JWT Token validated successfully");
                            return Task.CompletedTask;
                        }
                    };


                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration[WebConstants.Configuration.SECRET])),
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration[WebConstants.Configuration.ISSUER],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration[WebConstants.Configuration.AUDIENCE],
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.EnsureDB();
            app.FillTestUserData().GetAwaiter().GetResult();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<UserStatusMiddleware>();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
