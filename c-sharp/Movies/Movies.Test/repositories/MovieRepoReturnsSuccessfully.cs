using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.contracts;
using Movies.models;
using Movies.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using TestStack.BDDfy;

namespace Movies.Test.repositories
{
    [TestClass]
    public class MovieRepoReturnsSuccessfully
    {
        private MoviesRepository moviesRepo;
        private Mock<IHttpClientWrapper> mockHttpClient;
        private Mock<IConfiguration> mockConfig;

        private IList<MovieDto> result;

        [TestMethod]
        public void _MovieRepoReturnsSuccessfully()
        {
            new MovieRepoReturnsSuccessfully().BDDfy();
        }

        void GivenAMovieRepository()
        {
            mockHttpClient = new Mock<IHttpClientWrapper>();
            mockConfig = new Mock<IConfiguration>();

            mockConfig.Setup(config => config.BaseUri).Returns(new Uri("http://movies.com"));

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(@"[
  {
    ""name"": ""movie1"",
    ""roles"": [
      {
        ""name"": ""character1"",
        ""actor"": ""actor1""
      },
      {
        ""name"": ""character2"",
        ""actor"": ""actor2""
      }
    ]
  },
  {
    ""name"": ""movie2"",
    ""roles"": [
      {
        ""name"": ""character3"",
        ""actor"": ""actor1""
      }
    ]
  }
]");
            mockHttpClient.Setup(client => client.GetAsync(It.IsAny<Uri>())).ReturnsAsync(response);

            moviesRepo = new MoviesRepository(mockConfig.Object, mockHttpClient.Object);
        }

        void WhenMovieInformationIsRequested()
        {
            result = moviesRepo.GetMovies();
        }

        void ThenTheMoviesAreRequested()
        {
            mockHttpClient.Verify(client => client.GetAsync(It.Is<Uri>(uri => uri.AbsoluteUri == "http://movies.com/api/Movies")));
        }

        void ThenTheMovieWithActorAndRolesIsReturned()
        {
            result.Count.Should().Be(2);
            result.Select(movie => movie.Name).Should().Contain("movie1", "movie2");

            var movie1Roles = result.First(movie => movie.Name == "movie1").Roles;
            movie1Roles.Count().Should().Be(2);
            movie1Roles.Should().Equal(new List<RoleDto> {
                new RoleDto { Actor = "actor1", Name = "character1" },
                new RoleDto { Actor = "actor2", Name = "character2" }
            }, (role1, role2) => role1.Actor == role2.Actor && role1.Name == role2.Name);

            var movie2Roles = result.First(movie => movie.Name == "movie2").Roles;
            movie2Roles.Count().Should().Be(1);
            movie2Roles.Should().Equal(new List<RoleDto> {
                new RoleDto { Actor = "actor1", Name = "character3" },
            }, (role1, role2) => role1.Actor == role2.Actor && role1.Name == role2.Name);
        }
    }
}
