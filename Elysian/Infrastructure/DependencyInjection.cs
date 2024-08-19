using Elysian.Application.Interfaces;
using Elysian.Infrastructure.Context;
using Elysian.Infrastructure.Identity;
using Elysian.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Elysian.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ElysianContext>(options 
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IClaimsPrincipalAccessor, ClaimsPrincipalAccessor>();

            services.AddContentManagementFeatures(configuration);

            return services;
        }

        private static IServiceCollection AddContentManagementFeatures(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IWordPressService, WordPressService>(client =>
            {
                client.BaseAddress = configuration.GetValue<Uri>("WordPressCmsUri");
            });
            return services;
        }
    }
}
