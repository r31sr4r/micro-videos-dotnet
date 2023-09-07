using FC.Codeflix.Catalog.UnitTests.Common;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[CollectionDefinition(nameof(GenreTestFixture))]
public class GenreTestFixtureCollection
    : ICollectionFixture<GenreTestFixture>
{}

public class GenreTestFixture
    : BaseFixture
{
    public string GetValidGenreName()
        => Faker.Commerce.Categories(1)[0];

    public DomainEntity.Genre GetExampleGenre(
        bool isActive = true,
        List<Guid>? categoriesIdsList = null
    )
    {
        var genre = new DomainEntity.Genre(GetValidGenreName(), isActive);

        if ( categoriesIdsList is not null ) 
            foreach ( var categoryId in categoriesIdsList )
                genre.AddCategory( categoryId );

        return genre;

    }

}
