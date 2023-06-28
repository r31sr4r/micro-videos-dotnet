using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Xunit;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryTestFixture))]
public class DeleteCategoryTest
{
    private readonly DeleteCategoryTestFixture _fixture;
    public DeleteCategoryTest(DeleteCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task DeleteCategory()
    {
        var dbContext = _fixture.GetDbContext();
        var categoryExample = _fixture.GetValidCategory();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList(10));
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new ApplicationUseCases.DeleteCategory(
            repository,
            unitOfWork
        );

        var input = new DeleteCategoryInput(categoryExample.Id);
        
        await useCase.Handle(input, CancellationToken.None);

        var assertDbContext = _fixture.GetDbContext(true);
        var dbCategoryDeleted = await assertDbContext.Categories
            .FindAsync(categoryExample.Id);

        dbCategoryDeleted.Should().BeNull();
        var dbCategories = await assertDbContext.Categories.ToListAsync();
        dbCategories.Should().HaveCount(10);
    }

    [Fact(DisplayName = nameof(ThrowWhenCategoryNotFound))]
    [Trait("Integration/Application", "DeleteCategory - Use Cases")]
    public async Task ThrowWhenCategoryNotFound()
    {
        var dbContext = _fixture.GetDbContext();
        var categoryExample = _fixture.GetValidCategory();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList(10));
        var tracking = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        tracking.State = EntityState.Detached;
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.DeleteCategory(
                repository,
                unitOfWork
        );
        var input = new DeleteCategoryInput(Guid.NewGuid());

        Func<Task> act = async () => await useCase.Handle(input, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }
}
