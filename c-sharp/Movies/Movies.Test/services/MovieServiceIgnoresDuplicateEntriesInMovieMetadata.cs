using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.contracts;
using Movies.models;
using Movies.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestStack.BDDfy;

namespace Movies.Test.services
{
    [TestClass]
    public class MovieServiceIgnoresDuplicateEntriesInMovieMetadata
    {
        private MoviesService moviesService;
        private Mock<IMoviesRepository> mockRepo;

        private IList<ActorsWithRolesModel> result;

        [TestMethod]
        public void _MovieServiceIgnoresDuplicateEntriesInMovieMetadata()
        {
            new MovieServiceIgnoresDuplicateEntriesInMovieMetadata().BDDfy();
        }

        void GivenAMovieService()
        {
            mockRepo = new Mock<IMoviesRepository>();
            var moviesWithRoles = new List<MovieDto>
            {
                new MovieDto {
                    Name = "Rom Com",
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor Y", Name = "Finn"},
                        new RoleDto { Actor = "actor Q", Name = "HuckleBerry" },
                        new RoleDto { Actor = "actor Y", Name = "Finn"}
                    }
                },
                new MovieDto
                {
                    Name = "Comedy 1",
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor B", Name = "Giggles"},
                        new RoleDto { Actor = "actor Y", Name = "Laughs"},
                        new RoleDto { Actor = "actor Q", Name = "Smiles"},
                    }
                },
                new MovieDto
                {
                    Name = "Comedy 1",
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor B", Name = "Smirks"},
                        new RoleDto { Actor = "actor I", Name = "Grins"},
                        new RoleDto { Actor = "actor Q", Name = "Chuckles"},
                    }
                },
            };

            mockRepo.Setup(repo => repo.GetMovies()).Returns(moviesWithRoles);

            moviesService = new MoviesService(mockRepo.Object);
        }

        void WhenTheRolesPlayedByActorsIsRequested()
        {
            result = moviesService.GetRolesPlayedByActors();
        }

        void ThenTheMoviesAreRequested()
        {
            mockRepo.Verify(repo => repo.GetMovies(), Times.Once);
        }

        void ThenTheCorrectActorsAreReturned()
        {
            result.Count.Should().Be(4);
            result.Select(actor => actor.Name).Should().Contain(new List<string> { "actor B", "actor I", "actor Q", "actor Y" });
        }

        void ThenTheDuplicateRolesInTheSameFilmAreIgnored()
        {
            var actorYRoles = result.First(actor => actor.Name == "actor Y").Roles;
            actorYRoles.Count.Should().Be(2);
            actorYRoles.Should().Contain(new List<string> { "Laughs", "Finn" });
        }

        void ThenTheDuplicateFilmNamesArePreservedIfRolesAndActorsAreDifferent()
        {
            // allow duplicate film names with different roles and actors as the films could have different names
            // order of films will match order of movies in the list
            var actorBRoles = result.First(actor => actor.Name == "actor B").Roles;
            actorBRoles.Count.Should().Be(2);
            actorBRoles.Should().Contain(new List<string> { "Giggles", "Smirks" });

            var actorIRoles = result.First(actor => actor.Name == "actor I").Roles;
            actorIRoles.Count.Should().Be(1);
            actorIRoles.Should().Contain(new List<string> { "Grins" });

            var actorQRoles = result.First(actor => actor.Name == "actor Q").Roles;
            actorQRoles.Count.Should().Be(3);
            actorQRoles.Should().Contain(new List<string> { "Smiles", "Chuckles", "HuckleBerry" });
        }
    }
}
