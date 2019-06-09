using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using VueNote.Core.Util;
using VueNote.WebApi.Common;

namespace VueNote.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            ConfigHelper.Init(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ExceptionHandler>();
            services.AddCors(options =>
            {
                options.AddPolicy("mycors",
                builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "OPTIONS");
                    builder.WithHeaders("Content-Type", "Authorization");
                    foreach (var origin in ConfigHelper.AllowOrigins)
                    {
                        builder.WithOrigins(origin);
                    }
                });
            });
            services.AddAuthentication(option =>
            {
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // 过期时间允许误差设置为0（默认为5分钟）
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = ConfigHelper.JwtSettings.Issuer,
                    ValidAudience = ConfigHelper.JwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigHelper.JwtSettings.SigningKey))
                };
            });

            services.AddMvc(options =>
            {
                options.Filters.Add<ViewModelValidationFilter>();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration config)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseCors("mycors");
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller=Home}/{action=Index}");
            });
        }
    }
}
