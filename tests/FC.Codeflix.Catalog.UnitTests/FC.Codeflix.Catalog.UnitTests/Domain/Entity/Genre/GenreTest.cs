using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genrerName = "Horror";
        var datetimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genrerName);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genrerName);
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        genre.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genrerName = "Horror";
        var datetimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genrerName, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Name.Should().Be(genrerName);
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        genre.IsActive.Should().Be(isActive);
    }

}
