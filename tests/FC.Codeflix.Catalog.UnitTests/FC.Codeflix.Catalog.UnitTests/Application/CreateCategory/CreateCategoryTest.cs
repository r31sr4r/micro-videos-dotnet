using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.CreateCategory;
using FluentAssertions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Application.Interfaces;

namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTest
{
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "Create Category - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = new Mock<ICategoryRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var useCase = new UseCases.CreateCategory(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = new CreateCategoryInput(
                "Category Name",
                "Category Description",
                true
        );

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()
            ), 
            Times.Once
        );
        unitOfWorkMock.Verify( 
            unitOfWork =>  unitOfWork.Commit(It.IsAny<CancellationToken>()), 
            Times.Once 
        );
        output.Should().NotBeNull();
        output.Name.Should.Be("Category Name");
        output.Description.Should.Be("Category Description");
        output.IsActive.Should().BeTrue();
        (output.Id != null && output.Id != Guid.Empty).Should().BeTrue();
        (output.CreatedAt != null && output.CreatedAt != default(DateTime)).Should().BeTrue();        
    }
}
