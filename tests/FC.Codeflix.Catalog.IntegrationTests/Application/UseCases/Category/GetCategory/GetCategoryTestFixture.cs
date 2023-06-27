using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.GetCategory;

[CollectionDefinition(nameof(GetCategoryTestFixture))]
public class GetCategoryTestFixtureColletion
    : ICollectionFixture<GetCategoryTestFixture>
{ }

public class GetCategoryTestFixture 
    : CategoryUseCasesBaseFixture
{
}
