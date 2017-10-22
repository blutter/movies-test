using Movies.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.contracts
{
    public interface IMoviesRepository
    {
        IList<MovieDto> GetMovies();
    }
}
