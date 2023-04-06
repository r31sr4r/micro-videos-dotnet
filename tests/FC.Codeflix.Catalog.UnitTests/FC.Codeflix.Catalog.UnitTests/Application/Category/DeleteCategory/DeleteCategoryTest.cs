using FC.Codeflix.Catalog.Domain.Entity;
using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;

    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryExample = _fixture.GetValidCategory();
        repositoryMock.Setup(repository => repository.Get(
                    categoryExample.Id,
                    It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoryExample);
        var input = new UseCases.DeleteCategoryInput(categoryExample.Id);
        var useCase = new UseCases.DeleteCategory
            (repositoryMock.Object,
            unitOfWorkMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Get(
                categoryExample.Id,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        repositoryMock.Verify(
            repository => repository.Delete(
                categoryExample,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Once
        );




    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Application", "DeleteCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryGuid = Guid.NewGuid();
        repositoryMock.Setup(repository => repository.Get(
                    categoryGuid,
                    It.IsAny<CancellationToken>())
        ).ThrowsAsync(
            new NotFoundException($"Category '{categoryGuid}' not found")
        );
        var input = new UseCases.DeleteCategoryInput(categoryGuid);
        var useCase = new UseCases.DeleteCategory
            (repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                categoryGuid,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
