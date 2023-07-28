using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest
    : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;

    public void Dispose()
    => _fixture.CleanPersistence();

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await _fixture
            .ApiClient
            .Get<ListCategoriesOutput>("/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Items.Should().HaveCount(defaultPerPage);
        output.Page.Should().Be(1);
        output.PerPage.Should().Be(defaultPerPage);
        foreach (var item in output!.Items)
        {
            var exampleCategory = exampleCategoriesList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleCategory!.Name);
            item.Description.Should().Be(exampleCategory!.Description);
            item.IsActive.Should().Be(exampleCategory!.IsActive);
            item.CreatedAt.Should().BeSameDateAs(exampleCategory!.CreatedAt);
        }

    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture
            .ApiClient
            .Get<ListCategoriesOutput>("/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(0);
        output!.Items.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput
        {
            Page = 1,
            PerPage = 5
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<ListCategoriesOutput>("/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output!.Items.Should().HaveCount(input.PerPage);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var item in output!.Items)
        {
            var exampleCategory = exampleCategoriesList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleCategory!.Name);
            item.Description.Should().Be(exampleCategory!.Description);
            item.IsActive.Should().Be(exampleCategory!.IsActive);
            item.CreatedAt.Should().BeSameDateAs(exampleCategory!.CreatedAt);
        }

    }

}
