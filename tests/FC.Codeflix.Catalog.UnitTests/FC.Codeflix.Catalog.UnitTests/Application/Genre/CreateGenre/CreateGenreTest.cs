using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;
using FC.Codeflix.Catalog.Application;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Genre.CreateGenre;

[Collection(nameof(CreateGenreTestFixture))]

public class CreateGenreTest
{
    private readonly CreateGenreTestFixture _fixture;

    public CreateGenreTest(CreateGenreTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateGenre))]
    [Trait("Application", "Create Genre - Use Cases")]
    public async Task CreateGenre()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var input = _fixture.GetInput();
        var output = await useCase
            .Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(0);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateWithRelatedCategories))]
    [Trait("Application", "Create Genre - Use Cases")]
    public async Task CreateWithRelatedCategories()
    {
        var input = _fixture.GetInputWithCategories();
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        categoryRepositoryMock.Setup(
            repository => repository.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((IReadOnlyList<Guid>) input.CategoriesIds!);
        
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );


        var output = await useCase
            .Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Genre>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );
        output.Should().NotBeNull();
        output.Id.Should().NotBeEmpty();
        output.Name.Should().Be(input.Name);
        output.IsActive.Should().Be(input.IsActive);
        output.Categories.Should().HaveCount(input.CategoriesIds?.Count ?? 0);
        input.CategoriesIds?.ForEach(categoryId =>
                   output.Categories.Should().Contain(categoryId)
        );
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateThrowWhenRelatedCategoryNotFound))]
    [Trait("Application", "Create Genre - Use Cases")]
    public async Task CreateThrowWhenRelatedCategoryNotFound()
    {
        var input = _fixture.GetInputWithCategories();
        var exampleGuid = input.CategoriesIds![^1];
        var repositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        categoryRepositoryMock.Setup(
            repository => repository.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(
            (IReadOnlyList<Guid>) input.CategoriesIds
                .FindAll(id => id != exampleGuid)
        );
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var action = async () => await useCase
            .Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<RelatedAggregateException>()
            .WithMessage($"Related category id (or ids) not found: {exampleGuid}");

        categoryRepositoryMock.Verify(
            repository => repository.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsInvalid))]
    [Trait("Application", "Create Genre - Use Cases")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public async Task ThrowWhenNameIsInvalid(string name)
    {
        var input = _fixture.GetInput(name);
        var repositoryMock = _fixture.GetRepositoryMock();
        var categoryRepositoryMock = _fixture.GetCategoryRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();        
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            categoryRepositoryMock.Object
        );

        var action = async () => await useCase
            .Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<EntityValidationException>()
            .WithMessage($"Name cannot be empty or null");

    }
}
