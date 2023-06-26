using Bogus;
using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.IntegrationTests.Base;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public CodeflixCatalogDbContext GetDbContext(bool preserveData = false)
    {
        var context = new CodeflixCatalogDbContext(
             new DbContextOptionsBuilder<CodeflixCatalogDbContext>()
             .UseInMemoryDatabase("integration-tests-db")
             .Options
         );

        if (!preserveData)
            context.Database.EnsureDeleted();

        return context;
    }
}
