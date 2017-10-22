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
        private struct RoleInMovie {
            public string movieName;
            public string actorName;
            public string roleName;
        }

        public MoviesService(IMoviesRepository repository)
        {
            repo = repository;
        }

        public IList<ActorsWithRolesModel> GetRolesPlayedByActors()
        {
            var movies = repo.GetMovies();

            var actorsWithRoles = from roleInMovie in RemoveDuplicateMovieMetadata(movies)
                                  orderby roleInMovie.movieName
                                  group roleInMovie.roleName by roleInMovie.actorName;

            // Original implementation that handles the non-duplicate cases:
            //var actorsWithRoles = from movie in movies
            //                      from role in movie.Roles.Select(role => new { movieName = movie.Name, actorName = role.Actor, roleName = role.Name })
            //                      orderby movie.Name
            //                      group role.roleName by role.actorName;

            var result = actorsWithRoles.Select((group) => new ActorsWithRolesModel(group.Key, group.ToList()));

            return result.ToList();
        }

        private IList<RoleInMovie> RemoveDuplicateMovieMetadata(IList<MovieDto> movies)
        {
            var retVal = movies.SelectMany(movie => movie.Roles, (movie, role) => new RoleInMovie { movieName = movie.Name, actorName = role.Actor, roleName = role.Name })
                            .Distinct();
            return retVal.ToList<RoleInMovie>();
        }
    }
}
