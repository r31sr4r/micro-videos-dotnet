using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

[Collection(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTest
    : IDisposable
{
    private readonly UpdateCategoryApiTestFixture _fixture;
    public void Dispose()
        => _fixture.CleanPersistence();
    public UpdateCategoryApiTest(UpdateCategoryApiTestFixture fixture) => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    public async Task UpdateCategory()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var categoryModelInput = _fixture.GetExampleInput();

        var (response, output) = await _fixture
            .ApiClient
            .Put<ApiResponse<CategoryModelOutput>>(
            $"/categories/{exampleCategory.Id}", 
            categoryModelInput                        
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(categoryModelInput.Name);
        output.Data.Description.Should().Be(categoryModelInput.Description);
        output.Data.IsActive.Should().Be((bool)categoryModelInput.IsActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

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
        var input = new UpdateCategoryApiInput(
            _fixture.GetValidCategoryName()
        );

        var (response, output) = await _fixture
            .ApiClient
            .Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}", 
                input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(exampleCategory.Description);
        output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

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
        var input = new UpdateCategoryApiInput(
            _fixture.GetValidCategoryName(),
            _fixture.GetValidCategoryDescription()
        );

        var (response, output) = await _fixture
            .ApiClient
            .Put<ApiResponse<CategoryModelOutput>>(
                $"/categories/{exampleCategory.Id}",
                input
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleCategory.Id);
        output.Data.Name.Should().Be(input.Name);
        output.Data.Description.Should().Be(input.Description);
        output.Data.IsActive.Should().Be((bool)exampleCategory.IsActive!);
        output.Data.CreatedAt.Should().BeSameDateAs(exampleCategory.CreatedAt);

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
        var categoryModelInput = _fixture.GetExampleInput();

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

    [Theory(DisplayName = nameof(ErrorWhenCanNotInstantiateAggregate))]
    [Trait("EndToEnd/API", "Category/Update - Endpoints")]
    [MemberData(
        nameof(UpdateCategoryApiTestDataGenerator.GetInvalidInputs), 
        MemberType = typeof(UpdateCategoryApiTestDataGenerator)
    )]
    public async Task ErrorWhenCanNotInstantiateAggregate(
        UpdateCategoryApiInput input,
        string expectedDetail
        )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var exampleCategory = exampleCategoriesList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/categories/{exampleCategory.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors ocurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);

    }

}
