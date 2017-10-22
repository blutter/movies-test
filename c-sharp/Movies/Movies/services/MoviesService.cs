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

            // normalize the data to remove duplicates and empty or null entries - this should normally be a business decision
            var flattenedMovieMetadata = FlattenMovieMetadata(movies);
            var removedEmptyOrNullEntries = RemoveEmptyOrNullEntries(flattenedMovieMetadata);
            var removedDuplicates = RemoveDuplicateMovieMetadata(removedEmptyOrNullEntries);

            var actorsWithRoles = from roleInMovie in removedDuplicates
                                  orderby roleInMovie.movieName
                                  group roleInMovie.roleName by roleInMovie.actorName;

            // Original implementation that doesn't remove duplicates or empty entries:
            //var actorsWithRoles = from movie in movies
            //                      from role in movie.Roles.Select(role => new { movieName = movie.Name, actorName = role.Actor, roleName = role.Name })
            //                      orderby movie.Name
            //                      group role.roleName by role.actorName;

            var result = actorsWithRoles.Select((group) => new ActorsWithRolesModel(group.Key, group.ToList()));

            return result.ToList();
        }

        private IEnumerable<RoleInMovie> FlattenMovieMetadata(IEnumerable<MovieDto> movies)
        {
            var retVal = movies.SelectMany(movie => movie.Roles, (movie, role) => new RoleInMovie { movieName = movie.Name, actorName = role.Actor, roleName = role.Name });
            return retVal;
        }

        private IEnumerable<RoleInMovie> RemoveEmptyOrNullEntries(IEnumerable<RoleInMovie> movies)
        {
            var retVal = movies.Where(roleInMovie => 
                !string.IsNullOrWhiteSpace(roleInMovie.movieName) &&
                !string.IsNullOrWhiteSpace(roleInMovie.actorName) &&
                !string.IsNullOrWhiteSpace(roleInMovie.roleName));
            return retVal;
        }

        private IEnumerable<RoleInMovie> RemoveDuplicateMovieMetadata(IEnumerable<RoleInMovie> movies)
        {
            var retVal = movies.Distinct();
            return retVal;
        }
    }
}
