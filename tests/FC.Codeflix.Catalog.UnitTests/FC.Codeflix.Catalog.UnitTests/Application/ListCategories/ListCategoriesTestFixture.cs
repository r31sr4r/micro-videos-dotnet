using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection
    : ICollectionFixture<ListCategoriesTestFixture>
{}

public class ListCategoriesTestFixture 
    : BaseFixture
{
    public Mock<ICategoryRepository> GetRepositoryMock() => new();

    public string GetValidCategoryName()
    {
        var categoryName = "";
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];
        if (categoryName.Length > 255)
            categoryName = categoryName[..255];
        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        if (categoryDescription.Length > 10000)
            categoryDescription = categoryDescription[..10000];
        return categoryDescription;
    }
    public bool getRandomBoolean()
    => new Random().NextDouble() < 0.5;

    public Category GetValidCategory()
        => new(
            GetValidCategoryName(),
            GetValidCategoryDescription(),
            getRandomBoolean());

    public List<Category> GetCategoiesList(int lenght = 10)
    {
        var list = new List<Category>();
        for (int i = 0; i < lenght; i++)
        {
            list.Add(GetValidCategory());
        }
        return list;
    }

    public ListCategoriesInput GetInput()
    {
        var random = new Random();
        return new ListCategoriesInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Commerce.ProductName(),
            sort: Faker.Commerce.ProductName(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}
