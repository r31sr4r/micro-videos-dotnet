using FC.Codeflix.Catalog.IntegrationTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
public class CategoryUseCasesBaseFixture
    : BaseFixture
{
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

    public DomainEntity.Category GetValidCategory()
    => new(
        GetValidCategoryName(),
        GetValidCategoryDescription(),
        getRandomBoolean()
        );

    public List<DomainEntity.Category> GetCategoriesList(int lenght = 10)
    {
        return Enumerable.Range(1, lenght)
            .Select(_ => GetValidCategory()
        ).ToList();
    }

}
