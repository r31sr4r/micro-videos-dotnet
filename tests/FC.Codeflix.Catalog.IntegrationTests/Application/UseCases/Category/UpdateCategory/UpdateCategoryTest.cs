using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryTest
{
    private readonly UpdateCategoryTestFixture _fixture;

    public UpdateCategoryTest(UpdateCategoryTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Theory(DisplayName = nameof(UpdateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategory(
        DomainEntity.Category categoryExample,
        UpdateCategoryInput input
    )
    {
        var dbContext = _fixture.GetDbContext();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList());
        var trackingInfo = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new ApplicationUseCases.UpdateCategory(
            repository,
            unitOfWork
        );


        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.GetDbContext(true))
            .Categories
            .FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)input.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Theory(DisplayName = nameof(UpdateCategoryWithoutIsActive))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
    nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
    parameters: 5,
    MemberType = typeof(UpdateCategoryDataGenerator)
)]
    public async Task UpdateCategoryWithoutIsActive(
    DomainEntity.Category categoryExample,
    UpdateCategoryInput exampleInput
)
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Description
        );

        var dbContext = _fixture.GetDbContext();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList());
        var trackingInfo = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new ApplicationUseCases.UpdateCategory(
            repository,
            unitOfWork
        );


        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.GetDbContext(true))
            .Categories
            .FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be((bool)categoryExample.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)categoryExample.IsActive!);
    }

    [Theory(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
        nameof(UpdateCategoryDataGenerator.GetCategoriesToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateCategoryDataGenerator)
    )]
    public async Task UpdateCategoryOnlyName(
        DomainEntity.Category categoryExample,
        UpdateCategoryInput exampleInput
        )
    {
        var input = new UpdateCategoryInput(
            exampleInput.Id,
            exampleInput.Name
        );

        var dbContext = _fixture.GetDbContext();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList());
        var trackingInfo = await dbContext.AddAsync(categoryExample);
        await dbContext.SaveChangesAsync();
        trackingInfo.State = EntityState.Detached;

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new ApplicationUseCases.UpdateCategory(
            repository,
            unitOfWork
        );


        var output = await useCase.Handle(input, CancellationToken.None);

        var dbCategory = await (_fixture.GetDbContext(true))
            .Categories
            .FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(categoryExample.Description);
        dbCategory.IsActive.Should().Be((bool)categoryExample.IsActive!);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(categoryExample.Description);
        output.IsActive.Should().Be((bool)categoryExample.IsActive!);
    }

    [Fact(DisplayName = nameof(UpdateThrowsWhenCategoryNotFound))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    public async Task UpdateThrowsWhenCategoryNotFound()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.GetDbContext();
        await dbContext.AddRangeAsync(_fixture.GetCategoriesList());
        await dbContext.SaveChangesAsync();

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new ApplicationUseCases.UpdateCategory(
            repository,
            unitOfWork
        );

        var task = async () =>
            await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{input.Id}' not found.");
    }

    [Theory(DisplayName = nameof(UpdateThrowsWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "UpdateCategory - Use Cases")]
    [MemberData(
    nameof(UpdateCategoryDataGenerator.GetInvalidInputs),
    parameters: 5,
    MemberType = typeof(UpdateCategoryDataGenerator)
)]
    public async Task UpdateThrowsWhenCantInstantiateCategory(
        UpdateCategoryInput input,
        string expectedExceptionMessage
    )
    {
        var dbContext = _fixture.GetDbContext();
        var exampleCategories = _fixture.GetCategoriesList();
        await dbContext.AddRangeAsync(exampleCategories);
        await dbContext.SaveChangesAsync();

        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        input.Id = exampleCategories[0].Id;

        var useCase = new ApplicationUseCases.UpdateCategory(
            repository,
            unitOfWork
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

    }
}
    