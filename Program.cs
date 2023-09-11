
using Microsoft.AspNetCore.Identity;
using RealTimeChatApi.BusinessLogicLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Data;
using RealTimeChatApi.DataAccessLayer.Models;
using RealTimeChatApi.BusinessLogicLayer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RealTimeChatApi.DataAccessLayer.Interfaces;
using RealTimeChatApi.DataAccessLayer.Repositories;
using RealTimeChatApi.Middleware;
using Microsoft.AspNetCore.Authentication.Google;

namespace RealTimeChatApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add configuration from appsettings.json and other sources
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Configuration.AddEnvironmentVariables();



            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            //builder.Services.AddDbContext<RealTimeChatDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase("InMemoryDatabase"); // Provide a unique name
            //});
            var connectionString = builder.Configuration.GetConnectionString("RealTimeChatDbContext");
            builder.Services.AddDbContext<RealTimeChatDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });


            builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<RealTimeChatDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();




            builder.Services.AddScoped<IMessageService, MessageService>();
            IServiceCollection serviceCollection = builder.Services.AddScoped<IMessageRepository, MessageRepository>();

            builder.Services.AddTransient<ILogService, LogService>();
            builder.Services.AddTransient<ILogRepository, LogRepository>();


            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("It Is A Secret Key Which Should Not Be Shared With Other Users.....")),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            }
            );


            // Access configuration values
            var googleClientId = builder.Configuration["Google:ClientId"];
            var googleClientSecret = builder.Configuration["Google:ClientSecret"];
            builder.Services.AddAuthentication()
               .AddGoogle(googleoptions =>
               {
                   googleoptions.ClientId = googleClientId;
                   googleoptions.ClientSecret = googleClientSecret;
                   googleoptions.CallbackPath = "/auth/google-callback";
               });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseRequestLoggingMiddleware();


            app.MapControllers();

            app.Run();
        }
    }
}