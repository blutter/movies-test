using Movies.contracts;
using System;
using System.Collections.Generic;
using Movies.models;
using Newtonsoft.Json;

namespace Movies.repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly IConfiguration Config;
        private readonly IHttpClientWrapper HttpClient;

        private readonly Uri MoviesUri;

        public MoviesRepository(IConfiguration configuration, IHttpClientWrapper httpClient)
        {
            Config = configuration;
            HttpClient = httpClient;

            MoviesUri = new Uri(Config.BaseUri, "/api/Movies");
        }

        public IList<MovieDto> GetMovies()
        {
            var result = new List<MovieDto>();
            var response = HttpClient.GetAsync(MoviesUri).Result;

            if (response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<List<MovieDto>>(response.Content.ReadAsStringAsync().Result);
            }

            return result;
        }
    }
}
