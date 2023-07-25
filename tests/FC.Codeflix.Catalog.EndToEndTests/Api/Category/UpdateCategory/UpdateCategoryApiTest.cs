using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest
{
    private readonly UpdateCategoryApiTestFixture _fixture;
    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture) => _fixture = fixture;

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

    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void UpdateCategoryOnlyName()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryInput(
            exampleCategory.Id,
            _fixture.GetValidCategoryName()
        );

        var (response, output) = await _fixture
            .ApiClient
            .Put<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}", 
                input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(exampleCategory.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

        var category = await _fixture.Persistence.GetById(exampleCategory.Id);
        category.Should().NotBeNull();
        category!.Id.Should().Be(exampleCategory.Id);
        category.Name.Should().Be(input.Name);
        category.Description.Should().Be(exampleCategory.Description);
        category.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(UpdateCategoryNameAndDescription))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async void UpdateCategoryNameAndDescription()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];
        var input = new UpdateCategoryInput(
            exampleCategory.Id,
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var (response, output) = await _fixture
            .ApiClient
            .Put<CategoryModelOutput>(
                $"/categories/{exampleCategory.Id}",
                input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(exampleCategory.Id);
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        output.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

        var category = await _fixture.Persistence.GetById(exampleCategory.Id);
        category.Should().NotBeNull();
        category!.Id.Should().Be(exampleCategory.Id);
        category.Name.Should().Be(input.Name);
        category.Description.Should().Be(input.Description);
        category.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();
        var categoryModelInput = _fixture.GetExampleInput(randomGuid);

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/categories/{randomGuid}", categoryModelInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not found");
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
        output.Type.Should().Be("NotFound");
    }

}
