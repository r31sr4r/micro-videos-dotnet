﻿using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Genre;

[Collection(nameof(GenreTestFixture))]
public class GenreTest
{
    private readonly GenreTestFixture _fixture;

    public GenreTest(GenreTestFixture fixture)
        => this._fixture = fixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Instantiate()
    {
        var genrerName = _fixture.GetValidGenreName();
        var datetimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genrerName);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genrerName);
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        genre.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateThrowExceptionWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void InstantiateThrowExceptionWhenNameEmpty(string? name)
    {
        var action = new Action(() => new DomainEntity.Genre(name!));

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name cannot be empty or null");        
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        var genrerName = _fixture.GetValidGenreName();
        var datetimeBefore = DateTime.Now;

        var genre = new DomainEntity.Genre(genrerName, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        genre.Should().NotBeNull();
        genre.Id.Should().NotBeEmpty();
        genre.Name.Should().Be(genrerName);
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.CreatedAt.Should().BeOnOrAfter(datetimeBefore);
        genre.CreatedAt.Should().BeOnOrBefore(datetimeAfter);
        genre.IsActive.Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(Activate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Activate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive: isActive);

        genre.Activate();

        genre.Should().NotBeNull();
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void Deactivate(bool isActive)
    {
        var genre = _fixture.GetExampleGenre(isActive: isActive);

        genre.Deactivate();

        genre.Should().NotBeNull();
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Genre - Aggregates")]
    public void Update()
    {
        var genre = _fixture.GetExampleGenre();
        var newName = _fixture.GetValidGenreName();

        genre.Update(newName);       

        genre.Should().NotBeNull();
        genre.Name.Should().Be(newName);
        genre.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        genre.IsActive.Should().Be(genre.IsActive);
    }

    [Theory(DisplayName = nameof(UpdateThrowExceptionWhenNameEmpty))]
    [Trait("Domain", "Genre - Aggregates")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateThrowExceptionWhenNameEmpty(string? name)
    {
        var genre = _fixture.GetExampleGenre();

        var action = new Action(() => genre.Update(name!));

        action.Should().Throw<EntityValidationException>()
            .WithMessage("Name cannot be empty or null");
    }

    [Fact(DisplayName = nameof(AddCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddCategory()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryGuid = Guid.NewGuid();

        genre.AddCategory(categoryGuid);

        genre.Categories.Should().HaveCount(1);
        genre.Categories.Should().Contain(categoryGuid);
    }

    [Fact(DisplayName = nameof(AddTwoCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void AddTwoCategories()
    {
        var genre = _fixture.GetExampleGenre();
        var categoryGuid1 = Guid.NewGuid();
        var categoryGuid2 = Guid.NewGuid();

        genre.AddCategory(categoryGuid1);
        genre.AddCategory(categoryGuid2);


        genre.Categories.Should().HaveCount(2);
        genre.Categories.Should().Contain(categoryGuid1);
        genre.Categories.Should().Contain(categoryGuid2);
    }

    [Fact(DisplayName = nameof(RemoveCategory))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveCategory()
    {
        var exampleGuid = Guid.NewGuid();

        var genre = _fixture.GetExampleGenre(
            categoriesIdsList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                exampleGuid,
                Guid.NewGuid(),
                Guid.NewGuid()
            }    
        );

        genre.RemoveCategory(exampleGuid);

        genre.Categories.Should().HaveCount(4);
        genre.Categories.Should().NotContain(exampleGuid);
    }

    [Fact(DisplayName = nameof(RemoveAllCategories))]
    [Trait("Domain", "Genre - Aggregates")]
    public void RemoveAllCategories()
    {
        var genre = _fixture.GetExampleGenre(
            categoriesIdsList: new List<Guid>()
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            }
        );

        genre.RemoveAllCategories();

        genre.Categories.Should().HaveCount(0);
    }
}
