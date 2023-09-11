using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;
public  class GenreUseCasesBaseFixture
    : BaseFixture
{
    public Mock<IGenreRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

}
