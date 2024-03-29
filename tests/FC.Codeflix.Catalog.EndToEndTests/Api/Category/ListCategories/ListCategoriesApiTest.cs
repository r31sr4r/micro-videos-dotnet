﻿using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using FC.Codeflix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.Codeflix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using Xunit.Abstractions;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest
    : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public void Dispose()
    => _fixture.CleanPersistence();

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture, ITestOutputHelper output) 
        => (_fixture, _output) = (fixture, output);

    [Fact(DisplayName = nameof(ListCategoriesAndTotalByDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var (response, output) = await _fixture.ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>("/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Meta.CurrentPage.Should().Be(1);
        output.Meta.PerPage.Should().Be(defaultPerPage);
        output.Data.Should().HaveCount(defaultPerPage);
        foreach (CategoryModelOutput outputItem in output.Data)
        {
            var exampleItem = exampleCategoriesList
                .FirstOrDefault(x => x.Id == outputItem.Id);
            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.TrimMilliSeconds().Should().Be(
                exampleItem.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>("/categories");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(0);
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
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
            .Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output!.Data.Should().HaveCount(input.PerPage);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var item in output!.Data)
        {
            var exampleCategory = exampleCategoriesList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleCategory!.Name);
            item.Description.Should().Be(exampleCategory!.Description);
            item.IsActive.Should().Be(exampleCategory!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleCategory!.CreatedAt.TrimMilliSeconds()
            );
        }

    }

    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int totalCategoriesToGenerate,
        int page,
        int perPage,
        int expectedTotalItems
    )
    {
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(totalCategoriesToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput
        {
            Page = page,
            PerPage = perPage
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleCategoriesList.Count);
        output.Data.Should().HaveCount(expectedTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var item in output!.Data)
        {
            var exampleCategory = exampleCategoriesList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleCategory!.Name);
            item.Description.Should().Be(exampleCategory!.Description);
            item.IsActive.Should().Be(exampleCategory!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleCategory!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("No existent", 1, 3, 0, 0)]
    [InlineData("Sci-fi Robots", 1, 5, 1, 1)]
    public async Task SearchByText(
        string text,
        int page,
        int perPage,
        int expectedTotalItems,
        int expectedTotal
    )
    {
        var exampleCategoriesList = _fixture.GetCategoriesListWhitNames(new List<string>() {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi IA",
            "Sci-fi Space",
            "Sci-fi Robots",
            "Sci-fi Future",
        });
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput
        {
            Page = page,
            PerPage = perPage,
            Search = text
        };
        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<CategoryModelOutput>>("/categories", input);
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Meta.Total.Should().Be(expectedTotal);
        output.Data.Should().HaveCount(expectedTotalItems);
        output.Meta.CurrentPage.Should().Be(input.Page);
        output.Meta.PerPage.Should().Be(input.PerPage);
        foreach (var item in output!.Data)
        {
            var exampleCategory = exampleCategoriesList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleCategory!.Name);
            item.Description.Should().Be(exampleCategory!.Description);
            item.IsActive.Should().Be(exampleCategory!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleCategory!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    //[Theory(DisplayName = nameof(ListOrdered))]
    //[Trait("EndToEnd/API", "Category/List - Endpoints")]
    //[InlineData("name", "ASC")]
    //[InlineData("name", "DESC")]
    //[InlineData("id", "ASC")]
    //[InlineData("id", "DESC")]
    //[InlineData("createdAt", "ASC")]
    //[InlineData("createdAt", "DESC")]
    //[InlineData("", "asc")]
    //public async Task ListOrdered(
    //    string orderBy, 
    //    string order
    //)
    //{
    //    var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
    //    await _fixture.Persistence.InsertList(exampleCategoriesList);
    //    var inputOrder = order.ToUpper() == "ASC"
    //        ? SearchOrder.Asc
    //        : SearchOrder.Desc;
    //    var input = new ListCategoriesInput(
    //        page: 1,
    //        perPage: 20,
    //        sort: orderBy,
    //        dir: inputOrder
    //    );

    //    var (response, output) = await _fixture
    //        .ApiClient
    //        .Get<ListCategoriesOutput>("/categories", input);

    //    response.Should().NotBeNull();
    //    response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
    //    output.Should().NotBeNull();
    //    output!.Total.Should().Be(exampleCategoriesList.Count);
    //    output!.Items.Should().HaveCount(exampleCategoriesList.Count);
    //    output.Page.Should().Be(input.Page);
    //    output.PerPage.Should().Be(input.PerPage);

    //    var expectedOrderedList = _fixture.CloneAndOrderList(
    //        exampleCategoriesList,
    //        input.Sort,
    //        input.Dir
    //    );

    //    var count = 0;
    //    var expectedArr = expectedOrderedList.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");
    //    count = 0;
    //    var outputArr = output.Items.Select(x => $"{++count} {x.Name} {x.CreatedAt} {JsonConvert.SerializeObject(x)}");

    //    _output.WriteLine($"Expected: {string.Join("\n", expectedArr)}");
    //    _output.WriteLine($"Output: {string.Join("\n", outputArr)}");

    //    for (int i = 0; i < expectedOrderedList.Count; i++)
    //    {
    //        var expectedItem = expectedOrderedList[i];
    //        var outputItem = output.Items[i];
    //        expectedItem.Should().NotBeNull();
    //        outputItem.Should().NotBeNull();
    //        outputItem.Name.Should().Be(expectedItem!.Name);
    //        outputItem.Id.Should().Be(expectedItem.Id);
    //        outputItem.Description.Should().Be(expectedItem.Description);
    //        outputItem.IsActive.Should().Be(expectedItem.IsActive);            
    //        outputItem.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
    //            expectedItem.CreatedAt.TrimMilliSeconds()
    //        );
    //    }
    //}

}
