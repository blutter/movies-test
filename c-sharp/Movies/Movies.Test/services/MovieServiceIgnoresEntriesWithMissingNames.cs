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
    public class MovieServiceIgnoresEntriesWithMissingNames
    {
        private MoviesService moviesService;
        private Mock<IMoviesRepository> mockRepo;

        private IList<ActorsWithRolesModel> result;

        [TestMethod]
        public void _MovieServiceIgnoresEntriesWithMissingNames()
        {
            new MovieServiceIgnoresEntriesWithMissingNames().BDDfy();
        }

        void GivenAMovieService()
        {
            mockRepo = new Mock<IMoviesRepository>();
            var moviesWithRoles = new List<MovieDto>
            {
                new MovieDto {
                    Name = "Rom Com",
                    Roles = new List<RoleDto> {
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
                    Name = "SciFi",
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor A", Name = "Rockets"},
                        new RoleDto { Actor = "", Name = "Planets"},
                        new RoleDto { Actor = "actor Q", Name = ""},
                        new RoleDto { Actor = null, Name = "Space"},
                        new RoleDto { Actor = "actor L", Name = null},
                    }
                },
                new MovieDto
                {
                    Name = "",
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor B", Name = "Smirks"},
                        new RoleDto { Actor = "actor I", Name = "Grins"},
                        new RoleDto { Actor = "actor Q", Name = "Chuckles"},
                    }
                },
                new MovieDto
                {
                    Name = null,
                    Roles = new List<RoleDto> {
                        new RoleDto { Actor = "actor L", Name = "Smirks"},
                        new RoleDto { Actor = "actor M", Name = "Grins"},
                        new RoleDto { Actor = "actor N", Name = "Chuckles"},
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
            result.Select(actor => actor.Name).Should().Contain(new List<string> { "actor A", "actor B", "actor Q", "actor Y" });
        }

        void ThenTheRolesWithoutNamesAreIgnoredForEachActorAndSortedByFilmName()
        {
            // Note: movies without names are ignored

            var actorARoles = result.First(actor => actor.Name == "actor A").Roles;
            actorARoles.Count.Should().Be(1);
            actorARoles.Should().Contain(new List<string> { "Rockets" });

            var actorBRoles = result.First(actor => actor.Name == "actor B").Roles;
            actorBRoles.Count.Should().Be(1);
            actorBRoles.Should().Contain(new List<string> { "Giggles" });

            var actorQRoles = result.First(actor => actor.Name == "actor Q").Roles;
            actorQRoles.Count.Should().Be(2);
            actorQRoles.Should().Contain(new List<string> { "Smiles", "HuckleBerry" });

            var actorYRoles = result.First(actor => actor.Name == "actor Y").Roles;
            actorYRoles.Count.Should().Be(2);
            actorYRoles.Should().Contain(new List<string> { "Laughs", "Finn" });
        }
    }
}
