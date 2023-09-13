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
        GetRandomBoolean(),
        null
    );

    public CreateGenreInput GetInputWithCategories()
    {
        var numOfCategoriesIds = new Random().Next(1, 10);
        var categoriesIds = Enumerable.Range(1, numOfCategoriesIds)
            .Select(_ => Guid.NewGuid())
            .ToList();
        return new(
                GetValidGenreName(),
                GetRandomBoolean(),
                categoriesIds
            );
    }

    public Mock<IGenreRepository> GetRepositoryMock() => new();
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
    public Mock<ICategoryRepository> GetCategoryRepositoryMock() => new();
}
