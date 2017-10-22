using System;
using System.Linq;
using Autofac;
using Movies.contracts;

namespace MoviesDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            ContainerConfig.Setup();

            using (var scope = ContainerConfig.Container.BeginLifetimeScope())
            {
                var moviesService = scope.Resolve<IMoviesService>();

                var actorsAndRoles = moviesService.GetRolesPlayedByActors();

                actorsAndRoles.ToList().ForEach(actor =>
                {
                    Console.WriteLine(actor.Name);
                    actor.Roles.ToList().ForEach(role =>
                    {
                        Console.WriteLine($"  - {role}");
                    });
                });
            }

            Console.ReadLine();
        }
    }
}
