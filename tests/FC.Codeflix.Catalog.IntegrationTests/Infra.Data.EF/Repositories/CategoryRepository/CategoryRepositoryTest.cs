﻿using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.Exceptions;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

[Collection(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTest
{
    private readonly CategoryRepositoryTestFixture _fixture;

    public CategoryRepositoryTest(CategoryRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "ShouldInsertACategory")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldInsertACategory()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var category = _fixture.GetValidCategory();
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Insert(category, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (_fixture.GetDbContext())
            .Categories
            .FindAsync(category.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(category.Name);
        dbCategory.Description.Should().Be(category.Description);
        dbCategory.IsActive.Should().Be(category.IsActive);
        dbCategory.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = "ShouldGetACategory")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldGetACategory()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var category = _fixture.GetValidCategory();
        var catetoriesList = _fixture.GetCategoriesList(15);
        catetoriesList.Add(category);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
                
        var dbCategory = await categoryRepository.Get(category.Id, CancellationToken.None);

        dbCategory.Should().NotBeNull();
        dbCategory.Id.Should().Be(category.Id);
        dbCategory!.Name.Should().Be(category.Name);
        dbCategory.Description.Should().Be(category.Description);
        dbCategory.IsActive.Should().Be(category.IsActive);
        dbCategory.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = "GetShouldThrowIfNotFound")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task GetShouldThrowIfNotFound()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var categoryId = Guid.NewGuid();
        await dbContext.Categories.AddRangeAsync(_fixture.GetCategoriesList(15));
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(_fixture.GetDbContext());

        var task = async () => await categoryRepository.Get(
            categoryId, 
            CancellationToken.None
        );

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Category '{categoryId}' not found.");
    }

    [Fact(DisplayName = "ShouldUpdateACategory")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldUpdateACategory()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var category = _fixture.GetValidCategory();
        var newCategoryValues = _fixture.GetValidCategory();
        var catetoriesList = _fixture.GetCategoriesList(15);
        catetoriesList.Add(category);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        category.Update(newCategoryValues.Name, newCategoryValues.Description);
        await categoryRepository.Update(category, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (_fixture.GetDbContext())
            .Categories
            .FindAsync(category.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(category.Name);
        dbCategory.Description.Should().Be(category.Description);
        dbCategory.IsActive.Should().Be(category.IsActive);
        dbCategory.CreatedAt.Should().Be(category.CreatedAt);
    }
}