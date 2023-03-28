namespace FC.Codeflix.Catalog.UnitTests.Application.CreateCategory;
public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int numberOfIterations = 12)
    {
        var fixture = new CreateCategoryTestFixture();
        var invalidInputsList = new List<object[]>();
        var totalInvalidCases = 4;

        for(int index = 0; index < numberOfIterations; index++)
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
                    var invalidInputDescriptionNull = fixture.GetInput();
                    invalidInputDescriptionNull.Description = null!;
                    invalidInputsList.Add(new object[]
                    {
                        invalidInputDescriptionNull,
                        "Description cannot be null"
                    });
                    break;
                case 3:
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
