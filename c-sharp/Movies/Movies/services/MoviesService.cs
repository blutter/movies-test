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
                                  from role in movie.Roles
                                  select new { movieName = movie.Name, actorName = role.Actor, roleName = role.Name };
            var rolesGroupedByActors = from actorWithRole in actorsWithRoles
                                       orderby actorWithRole.movieName
                                       group actorWithRole.roleName by actorWithRole.actorName;

            var result = rolesGroupedByActors.Select((group) => new ActorsWithRolesModel(group.Key, group.ToList()));

            return result.ToList();
        }
    }
}
