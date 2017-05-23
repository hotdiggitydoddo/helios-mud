using System;
using Helios.Data;
using Helios.Domain.Contracts;
using Helios.Domain.Models;
using Helios.Engine.Factories;
using Helios.Engine.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Helios.Engine;
using Helios.Engine.UI;

namespace Helios.ConsoleApp
{
    class Program
    {
        private HeliosConsole _aaConsole;
        private Game _game;
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<HeliosDbContext>(options =>
                {
                    options.UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=HeliosDb;Integrated Security=True;Persist Security Info=False");
                })
                .AddTransient<IRepository<Account>, Repository<Account>>()
                .AddTransient<IRepository<Entity>, Repository<Entity>>()
                .AddTransient<IRepository<Trait>, Repository<Trait>>()
                .AddTransient<IEntityFactory, EntityFactory>()
                .AddTransient<IOutputFormatter, OutputConsole>()
                .AddHeliosGame()
                .BuildServiceProvider();
            
            var game = serviceProvider.GetService<Game>();

            var aaConsole = new HeliosConsole();
            aaConsole.Init();
            Console.WriteLine("Press a key to login");
            Console.ReadLine();
            aaConsole.Login();
            Console.WriteLine("Exiting...");
            Console.ReadLine();
        }
    }
}
