using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchebleRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryInput input,
        CancellationToken cancellation
    )
    {
        var output = await _mediator.Send(input);
        return CreatedAtAction(
            nameof(Create), 
            new { output.Id }, 
            output
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellation
    )
    {
        var output = await _mediator.Send(new GetCategoryInput(id), cancellation);
        return Ok(output);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellation
    )
    {
        await _mediator.Send(new DeleteCategoryInput(id), cancellation);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateCategoryInput input,
        CancellationToken cancellation
    )
    {
        input.Id = id;
        var output = await _mediator.Send(input, cancellation);
        return Ok(output);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListCategoriesOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListCategoriesInput();
        if (page.HasValue)
            input.Page = page.Value;
        if (perPage.HasValue)
            input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search))
            input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort))
            input.Sort = sort;
        if (dir.HasValue)
            input.Dir = dir.Value;


        var output = await _mediator.Send(input, cancellation);
        return Ok(output);
    }

}
