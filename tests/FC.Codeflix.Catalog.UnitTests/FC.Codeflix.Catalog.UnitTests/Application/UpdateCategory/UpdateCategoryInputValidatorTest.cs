using Xunit;
using FluentAssertions;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.UpdateCategory;

[Collection(nameof(UpdateCategoryTestFixture))]
public class UpdateCategoryInputValidatorTest
{
    private readonly UpdateCategoryTestFixture fixture;

    public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DontAcceptWhenGuidIsEmpty))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void DontAcceptWhenGuidIsEmpty()
    {
        var input = fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult.Errors[0].ErrorMessage
            .Should().Be("Id must not be empty");
    }

    [Fact(DisplayName = nameof(AcceptWhenGuidIsNotEmpty))]
    [Trait("Application", "UpdateCategoryInputValidator - Use Cases")]
    public void AcceptWhenGuidIsNotEmpty()
    {
        var input = fixture.GetValidInput();
        var validator = new UpdateCategoryInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }
}
