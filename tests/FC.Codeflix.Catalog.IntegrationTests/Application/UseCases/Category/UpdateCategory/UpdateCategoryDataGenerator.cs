namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.UpdateCategory;
public class UpdateCategoryDataGenerator
{
    public static IEnumerable<object[]> GetCategoriesToUpdate(int times = 10)
    {
        var fixture = new UpdateCategoryTestFixture();
        for (int indice = 0; indice < times; indice++)
        {
            var exampleCategory = fixture.GetValidCategory();
            var exampleInput = fixture.GetValidInput(exampleCategory.Id);
            yield return new object[] { exampleCategory, exampleInput };
        }
    }

    public static IEnumerable<object[]> GetInvalidInputs(int numberOfIterations = 12)
    {
        var fixture = new UpdateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 3;

        for (int index = 0; index < numberOfIterations; index++)
        {
            switch (index % totalInvalidCases)
            {
                case 0:
                    invalidInputsList.Add(new object[]
                    {
                    fixture.GetInvalidInputShortName(),
                    "Name should be at least 3 characters long"
                });
                    break;
                case 1:
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputTooLongName(),
                        "Name should not be greater than 255 characters long"
                    });
                    break;
                case 2:
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputDescriptionTooLong(),
                        "Description should not be greater than 10000 characters long"
                    });
                    break;
            }
        }

        return invalidInputsList;
    }
}
