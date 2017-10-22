using Movies.contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Movies.models;
using System.Linq;

namespace Movies.services
{
    public class MoviesService : IMoviesService
    {
        private readonly IMoviesRepository repo;

        public MoviesService(IMoviesRepository repository)
        {
            repo = repository;
        }

        public IList<ActorsWithRolesModel> GetRolesPlayedByActors()
        {
            var movies = repo.GetMovies();

            var actorsWithRoles = from movie in movies
                                  from role in movie.Roles.Select(role => new { movieName = movie.Name, actorName = role.Actor, roleName = role.Name })
                                  orderby movie.Name
                                  group role.roleName by role.actorName;

            var result = actorsWithRoles.Select((group) => new ActorsWithRolesModel(group.Key, group.ToList()));

            return result.ToList();
        }
    }
}
