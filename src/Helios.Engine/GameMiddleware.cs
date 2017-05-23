using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Helios.Engine
{
    public class GameMiddleware
    {
         private readonly RequestDelegate _next;
        private Game _game { get; set; }

        public GameMiddleware(RequestDelegate next, Game game)
        {
            _next = next;
            _game = game;
        }

         public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
        }
    }
}