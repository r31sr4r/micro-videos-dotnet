using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.UnitTests.Application.Genre.Common;

using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[CollectionDefinition(nameof(CreateGenreTestFixture))]
public class CreateGenreTestFixtureCollection
    : ICollectionFixture<CreateGenreTestFixture>
{ }

public class CreateGenreTestFixture 
    : GenreUseCasesBaseFixture
{

    public DomainEntity.Genre GetInput()
    => new(
        GetValidGenreName(),
        GetRandomBoolean()
    );
}
