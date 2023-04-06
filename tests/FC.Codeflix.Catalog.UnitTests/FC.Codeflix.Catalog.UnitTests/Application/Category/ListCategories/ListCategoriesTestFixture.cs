using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using FC.Codeflix.Catalog.UnitTests.Application.Category.Common;
using Moq;
using Xunit;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.ListCategories;

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection
    : ICollectionFixture<ListCategoriesTestFixture>
{ }

public class ListCategoriesTestFixture
    : CategoryUseCasesBaseFixture
{
    public List<DomainEntity.Category> GetCategoiesList(int lenght = 10)
    {
        var list = new List<DomainEntity.Category>();
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
