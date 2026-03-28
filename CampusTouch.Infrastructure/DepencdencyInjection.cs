using CampusTouch.Application.Interfaces;
using CampusTouch.Infrastructure.Identity;
using CampusTouch.Infrastructure.Persistance.Identity;
using CampusTouch.Infrastructure.Persistance.Repositories;
using CampusTouch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;


namespace CampusTouch.Infrastructure
{
    public static  class DepencdencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration) 
        
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

           
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ICurrentUserService,CurrentUserServices>();
            services.AddScoped<IDepartementRepository,DepartementRepository>();


            services.AddScoped<IJWTService,JwtServices>();
            services.AddScoped<IDbConnection>(sp =>
            new SqlConnection(configuration.GetConnectionString("DefaultConnection")));


            return services;


        }
    }
}
    