using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest
{
    private readonly UpdateCategoryApiTestFixture _fixture;
    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var categoryModelInput = _fixture.GetExampleInput(exampleCategory.Id);

        var (response, output) = await _fixture
            .ApiClient
            .Put<CategoryModelOutput>($"/categories/{exampleCategory.Id}", categoryModelInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(categoryModelInput.Name);
        output.Description.Should().Be(categoryModelInput.Description);
        output.IsActive.Should().Be((bool)categoryModelInput.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

        var category = await _fixture.Persistence.GetById(exampleCategory.Id);
        category.Should().NotBeNull();
        category!.Id.Should().Be(exampleCategory.Id);
        category.Name.Should().Be(categoryModelInput.Name);
        category.Description.Should().Be(categoryModelInput.Description);
        category.IsActive.Should().Be((bool)categoryModelInput.IsActive!);
        category.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);
    }
}
