using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using FC.Codeflix.Catalog.Domain.Entity;

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

        var dbCategory = await (_fixture.GetDbContext(true))
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

        var dbCategory = await (_fixture.GetDbContext(true))
            .Categories
            .FindAsync(category.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(category.Name);
        dbCategory.Description.Should().Be(category.Description);
        dbCategory.IsActive.Should().Be(category.IsActive);
        dbCategory.CreatedAt.Should().Be(category.CreatedAt);
    }

    [Fact(DisplayName = "ShouldDeleteACategory")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldDeleteACategory()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var category = _fixture.GetValidCategory();
        var catetoriesList = _fixture.GetCategoriesList(15);
        catetoriesList.Add(category);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);

        await categoryRepository.Delete(category, CancellationToken.None);
        await dbContext.SaveChangesAsync();

        var dbCategory = await (_fixture.GetDbContext(true))
            .Categories
            .FindAsync(category.Id);

        dbCategory.Should().BeNull();
    }

    [Fact(DisplayName = "ShouldSearchAndReturnsListAndTotal")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldSearchAndReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var catetoriesList = _fixture.GetCategoriesList(15);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
        
        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(catetoriesList.Count);
        output.Items.Should().HaveCount(catetoriesList.Count);
        foreach(Category outputItem in output.Items)
        {
            var item = catetoriesList.Find(
                category => category.Id == outputItem.Id
            );

            item.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(item!.Name);
            outputItem.Description.Should().Be(item.Description);
            outputItem.IsActive.Should().Be(item.IsActive);
            outputItem.CreatedAt.Should().Be(item.CreatedAt);
        }
    }

    [Fact(DisplayName = "ShouldSearchAndReturnsEmpty")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    public async Task ShouldSearchAndReturnsEmpty()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = "ShouldSearchAndReturnsPaginatedList")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ShouldSearchAndReturnsPaginatedList(
        int totalCategoriesToGenerate,
        int page,
        int perPage,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var catetoriesList = _fixture.GetCategoriesList(totalCategoriesToGenerate);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(totalCategoriesToGenerate);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (Category outputItem in output.Items)
        {
            var item = catetoriesList.Find(
                category => category.Id == outputItem.Id
            );

            item.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(item!.Name);
            outputItem.Description.Should().Be(item.Description);
            outputItem.IsActive.Should().Be(item.IsActive);
            outputItem.CreatedAt.Should().Be(item.CreatedAt);
        }
    }

    [Theory(DisplayName = "ShouldSearchByTextAndReturnsList")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("No existent", 1, 3, 0, 0)]
    [InlineData("Sci-fi Robots", 1, 5, 1, 1)]
    public async Task ShouldSearchByTextAndReturnsList(
        string search,
        int page,
        int perPage,
        int expectedTotalItemsReturned,
        int expectedTotalItems
    )
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var catetoriesList = _fixture.GetCategoriesListWhitNames(new List<string>() {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });

        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchInput = new SearchInput(page, perPage, search, "", SearchOrder.Asc);

        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedTotalItemsReturned);
        foreach (Category outputItem in output.Items)
        {
            var item = catetoriesList.Find(
                category => category.Id == outputItem.Id
            );

            item.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(item!.Name);
            outputItem.Description.Should().Be(item.Description);
            outputItem.IsActive.Should().Be(item.IsActive);
            outputItem.CreatedAt.Should().Be(item.CreatedAt);
        }
    }


    [Theory(DisplayName = "ShouldSearchOrderedAndReturnsList")]
    [Trait("Integration/Infra.Data", "CategoryRepository - Repositories")]
    [InlineData("name", "ASC")]
    [InlineData("name", "DESC")]
    [InlineData("id", "ASC")]
    [InlineData("id", "DESC")]
    [InlineData("createdAt", "ASC")]
    [InlineData("createdAt", "DESC")]
    [InlineData("", "asc")]
    public async Task ShouldSearchOrderedAndReturnsList(
    string orderBy,
    string order)
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var catetoriesList = _fixture.GetCategoriesList(10);

        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new Repository.CategoryRepository(dbContext);
        var searchOrder = order.ToUpper() == "ASC" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(1, 20, "", orderBy, searchOrder);

        var output = await categoryRepository.Search(searchInput, CancellationToken.None);

        var expectedOrderedList = _fixture.CloneAndOrderList(
            catetoriesList, 
            orderBy, 
            searchOrder
        );

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(catetoriesList.Count);
        output.Items.Should().HaveCount(catetoriesList.Count);
        for(int i = 0;  i < expectedOrderedList.Count; i++)
        {
            var expectedItem = expectedOrderedList[i];
            var outputItem = output.Items[i];
            expectedItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(expectedItem!.Name);
            outputItem.Id.Should().Be(expectedItem.Id);
            outputItem.Description.Should().Be(expectedItem.Description);
            outputItem.IsActive.Should().Be(expectedItem.IsActive);
            outputItem.CreatedAt.Should().Be(expectedItem.CreatedAt);
        }
    }
}
