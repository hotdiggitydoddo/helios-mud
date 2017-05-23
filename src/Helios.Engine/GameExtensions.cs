using System.Reflection;
using Helios.Engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Helios.Engine
{
    public static class GameExtensions
    {
        public static IServiceCollection AddHeliosGame(this IServiceCollection services)
        {
            services.AddTransient<Game>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(Game))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }

        public static IApplicationBuilder UseHelios(this IApplicationBuilder app,
                                                              Game game)
        {
            return app.UseMiddleware<GameMiddleware>(game);
        }
    }
}
