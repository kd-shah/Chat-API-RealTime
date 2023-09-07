
using Microsoft.AspNetCore.Identity;
using RealTimeChatApi.DataAccessLayer.Data;

namespace RealTimeChatApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            //.AddEntityFrameworkStores<RealTimeChatDbContext>()
            //.AddDefaultTokenProviders();
            //builder.Services.AddScoped<UserManager<IdentityUser>>();
            //builder.Services.AddScoped<SignInManager<IdentityUser>>();

            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
        .AddEntityFrameworkStores<RealTimeChatDbContext>()
        .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}