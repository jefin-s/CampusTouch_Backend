using CampusTouch.Application.Interfaces;
using CampusTouch.Infrastructure.Identity;
using CampusTouch.Infrastructure.Persistance.Identity;
using CampusTouch.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


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

            services.AddScoped<IJWTService,JwtServices>();

            return services;


        }
    }
}
