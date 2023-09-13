using Moq;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Genre.CreateGenre;
using FluentAssertions;

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
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object
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
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateGenre(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = _fixture.GetInputWithCategories();

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

}
