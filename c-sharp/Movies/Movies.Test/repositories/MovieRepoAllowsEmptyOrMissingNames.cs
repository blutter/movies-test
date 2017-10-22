using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.contracts;
using Movies.models;
using Movies.repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class MovieRepoAllowsEmptyOrMissingNames
    {
        private MoviesRepository moviesRepo;
        private Mock<IHttpClientWrapper> mockHttpClient;
        private Mock<IConfiguration> mockConfig;

        private IList<MovieDto> result;

        [TestMethod]
        public void _MovieRepoAllowsEmptyOrMissingNames()
        {
            new MovieRepoAllowsEmptyOrMissingNames().BDDfy();
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
        ""actor"": """"
      },
      {
        ""name"": ""character3""
      }
    ]
  },
  {
    ""name"": ""movie2"",
    ""roles"": [
      {
        ""name"": """",
        ""actor"": ""actor1""
      },
      {
        ""actor"": ""actor2""
      },
      {
        ""name"": ""character4"",
        ""actor"": ""  ""
      }
    ]
  },
  {
    ""name"": """",
    ""roles"": [
      {
        ""name"": ""character5"",
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
            result.Count.Should().Be(3);
            result.Select(movie => movie.Name).Should().Contain("movie1", "movie2", "");

            var movie1Roles = result.First(movie => movie.Name == "movie1").Roles;
            movie1Roles.Count().Should().Be(3);
            movie1Roles.Should().Equal(new List<RoleDto> {
                new RoleDto { Actor = "actor1", Name = "character1" },
                new RoleDto { Actor = "", Name = "character2" },
                new RoleDto { Actor = null, Name = "character3" }
            }, (role1, role2) => role1.Actor == role2.Actor && role1.Name == role2.Name);

            var movie2Roles = result.First(movie => movie.Name == "movie2").Roles;
            movie2Roles.Count().Should().Be(3);
            movie2Roles.Should().Equal(new List<RoleDto> {
                new RoleDto { Actor = "actor1", Name = "" },
                new RoleDto { Actor = "actor2", Name = null },
                new RoleDto { Actor = "  ", Name = "character4" },
            }, (role1, role2) => role1.Actor == role2.Actor && role1.Name == role2.Name);

            var movie3Roles = result.First(movie => movie.Name == "").Roles;
            movie3Roles.Count().Should().Be(1);
            movie3Roles.Should().Equal(new List<RoleDto> {
                new RoleDto { Actor = "actor1", Name = "character5" },
            }, (role1, role2) => role1.Actor == role2.Actor && role1.Name == role2.Name);
        }
    }
}
