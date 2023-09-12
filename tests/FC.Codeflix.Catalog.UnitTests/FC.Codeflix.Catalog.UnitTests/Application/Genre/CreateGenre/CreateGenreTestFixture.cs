using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

using Xunit;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using Moq;
using FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection
    : ICollectionFixture<CreateGenreTestFixture>
{ }

public class CreateGenreTestFixture 
    : GenreUseCasesBaseFixture
{

    public CreateGenreInput GetInput()
    => new(
        GetValidGenreName(),
        GetRandomBoolean()
    );

    public Mock<IGenreRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}
