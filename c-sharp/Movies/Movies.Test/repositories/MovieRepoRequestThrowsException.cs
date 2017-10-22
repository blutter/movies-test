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
    public class MovieRepoApiRequestThrowsException
    {
        private MoviesRepository moviesRepo;
        private Mock<IHttpClientWrapper> mockHttpClient;
        private Mock<IConfiguration> mockConfig;

        private IList<MovieDto> result;
        private Exception exception;

        [TestMethod]
        public void _MovieRepoApiRequestThrowsException()
        {
            new MovieRepoApiRequestThrowsException().BDDfy();
        }

        void GivenAMovieRepository()
        {
            mockHttpClient = new Mock<IHttpClientWrapper>();
            mockConfig = new Mock<IConfiguration>();

            mockConfig.Setup(config => config.BaseUri).Returns(new Uri("http://movies.com"));

            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            mockHttpClient.Setup(client => client.GetAsync(It.IsAny<Uri>())).Throws(new Exception("oh oh"));

            moviesRepo = new MoviesRepository(mockConfig.Object, mockHttpClient.Object);
        }

        void WhenMovieInformationIsRequested()
        {
            try
            {
                result = moviesRepo.GetMovies();
            }
            catch (Exception e)
            {
                exception = e;
            }
        }

        void ThenTheMoviesAreRequested()
        {
            mockHttpClient.Verify(client => client.GetAsync(It.Is<Uri>(uri => uri.AbsoluteUri == "http://movies.com/api/Movies")));
        }

        void ThenTheExceptionIsRethrown()
        {
            exception.Should().NotBeNull();
            exception.Message.Should().Contain("oh oh");
        }
    }
}
