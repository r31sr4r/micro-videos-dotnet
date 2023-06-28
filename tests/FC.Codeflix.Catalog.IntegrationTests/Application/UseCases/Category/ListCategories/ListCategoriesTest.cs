using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Xunit;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;


namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

[Collection(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTest
{
    private readonly ListCategoriesTestFixture _fixture;

    public ListCategoriesTest(ListCategoriesTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "ShouldSearchAndReturnsListAndTotal")]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task ShouldSearchAndReturnsListAndTotal()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var catetoriesList = _fixture.GetCategoriesList(15);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1, 20);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(catetoriesList.Count);
        output.Items.Should().HaveCount(catetoriesList.Count);
        foreach (CategoryModelOutput outputItem in output.Items)
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

    [Fact(DisplayName = "ShouldSearchAndReturnsEmptyList")]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
    public async Task ShouldSearchAndReturnsEmptyList()
    {
        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(1, 20);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

    }

    [Theory(DisplayName = "ShouldSearchAndReturnsPaginatedList")]
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page, perPage);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(catetoriesList.Count);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (CategoryModelOutput outputItem in output.Items)
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
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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

        CodeflixCatalogDbContext dbContext = _fixture.GetDbContext();
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var searchInput = new ListCategoriesInput(page, perPage, search);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalItems);
        output.Items.Should().HaveCount(expectedTotalItemsReturned);
        foreach (CategoryModelOutput outputItem in output.Items)
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
    [Trait("Integration/Application", "ListCategories - Use Cases")]
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
        var catetoriesList = _fixture.GetCategoriesList(15);
        await dbContext.Categories.AddRangeAsync(catetoriesList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var categoryRepository = new CategoryRepository(dbContext);
        var useCase = new ApplicationUseCases.ListCategories(categoryRepository);

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
        for (int i = 0; i < expectedOrderedList.Count; i++)
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
