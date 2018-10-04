using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Movies.contracts;
using Movies.models;
using Movies.services;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Movies.Test.services
{
    [TestClass]
    public class MovieServiceReturnsRolesForActors
    {
        private MoviesService moviesService;
        private Mock<IMoviesRepository> mockRepo;

        private IList<ActorsWithRolesModel> result;

        [TestMethod]
        public void _MovieServiceReturnsRolesForActors()
        {
            new MovieServiceReturnsRolesForActors().BDDfy();
        }

        void GivenAMovieService()
        {
            mockRepo = new Mock<IMoviesRepository>();
            var moviesWithRoles = new List<MovieDto>
            {
                Builder<MovieDto>.CreateNew()
                    .With(dto => dto.Name = "Rom Com")
                    .And(dto => dto.Roles = Builder<RoleDto>.CreateListOfSize(2)
                        .TheFirst(1).With(role => role.Actor = "actor Q").And(role => role.Name = "HuckleBerry")
                        .TheNext(1).With(role => role.Actor = "actor Y").And(role => role.Name = "Finn")
                        .Build())
                    .Build(),
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
                        new RoleDto { Actor = "actor I", Name = "Planets"},
                        new RoleDto { Actor = "actor Q", Name = "Stars"},
                    }
                },
                new MovieDto
                {
                    Name = "Comedy 2",
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
            result.Count.Should().Be(5);
            result.Select(actor => actor.Name).Should().Contain(new List<string> { "actor A", "actor B", "actor I", "actor Q", "actor Y" });
        }

        void ThenTheRolesForEachActorAreSortedByFilmName()
        {
            var actorARoles = result.First(actor => actor.Name == "actor A").Roles;
            actorARoles.Count.Should().Be(1);
            actorARoles.Should().Contain(new List<string> { "Rockets" });

            var actorBRoles = result.First(actor => actor.Name == "actor B").Roles;
            actorBRoles.Count.Should().Be(2);
            actorBRoles.Should().Contain(new List<string> { "Giggles", "Smirks" });

            var actorIRoles = result.First(actor => actor.Name == "actor I").Roles;
            actorIRoles.Count.Should().Be(2);
            actorIRoles.Should().Contain(new List<string> { "Grins", "Planets" });

            var actorQRoles = result.First(actor => actor.Name == "actor Q").Roles;
            actorQRoles.Count.Should().Be(4);
            actorQRoles.Should().Contain(new List<string> { "Smiles", "Chuckles", "HuckleBerry", "Stars" });

            var actorYRoles = result.First(actor => actor.Name == "actor Y").Roles;
            actorYRoles.Count.Should().Be(2);
            actorYRoles.Should().Contain(new List<string> { "Laughs", "Finn" });
        }
    }
}
