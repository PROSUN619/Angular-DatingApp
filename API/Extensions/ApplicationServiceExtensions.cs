using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudiniarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService,TokenService>();
            // services.AddScoped<IUserRepository,UserRepository>();
            services.AddScoped<IPhotoService,PhotoService>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options => {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });    
            services.AddScoped<LogUserActivity>();//user this acttion in the base api controller to work for same
            // services.AddScoped<ILikesRepository, LikesRepository>();
            // services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            
            return services;
        }
    }
}