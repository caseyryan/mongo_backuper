using System;
using System.Text;
using Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Services;

namespace MongoBackuper
{
    public class Startup
    {
        
        public IConfiguration Configuration { get; private set; }
        public IHostingEnvironment Environment { get; private set; }


        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services) 
        {

            var jwtSecretBytes = Encoding.ASCII.GetBytes(Configuration["JwtSecret"]);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAuthentication(options => 
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options => {
                    // сам токен генерируется в JwtTokenProvider
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters 
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes),
                        ValidateLifetime = false,
                        RequireExpirationTime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = Configuration["Issuer"],
                        ValidAudience = Configuration["Audience"],
                    };
                });
            // services.AddHostedService<BackupService>();
            services.AddSingleton<BackupService>();
            services.AddHostedService<BackupService>(provider => provider.GetService<BackupService>());

            services.AddSingleton<ITokenService, JwtTokenService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Environment = env;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
            
        }
    }
}
