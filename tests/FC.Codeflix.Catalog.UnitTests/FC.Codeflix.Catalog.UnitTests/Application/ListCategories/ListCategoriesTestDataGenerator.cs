using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using System.Reflection.Metadata.Ecma335;

namespace FC.Codeflix.Catalog.UnitTests.Application.ListCategories;
public class ListCategoriesTestDataGenerator
{
    public static IEnumerable<object[]> GetInputWithoutAllParameters(int times = 18)
    {
        var fixture = new ListCategoriesTestFixture();
        var inputExample = fixture.GetInput();
        for (int i = 0; i < times; i++)
        {
            switch (i % 6)
            {
                case 0:
                    yield return new object[] { new ListCategoriesInput() };
                    break;
                case 1: 
                    yield return new object[] { new ListCategoriesInput(inputExample.Page) };
                    break;
                case 2:
                    yield return new object[] { new ListCategoriesInput(
                        inputExample.Page,
                        inputExample.PerPage
                        ) };
                    break;
                case 3:
                    yield return new object[] { new ListCategoriesInput(
                        inputExample.Page,
                        inputExample.PerPage,
                        inputExample.Search
                        ) };
                    break;
                case 4:
                    yield return new object[] { new ListCategoriesInput(
                        inputExample.Page,
                        inputExample.PerPage,
                        inputExample.Search,
                        inputExample.Sort
                        ) };
                    break;
                case 5:
                    yield return new object[] { inputExample };
                    break;

                default:
                    yield return new object[] { new ListCategoriesInput() };
                    break;
            }
        }
    }
}
