using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
public class GetCategory : IGetCategory
{
    private readonly ICategoryRepository _repository;

    public GetCategory(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryModelOutput> Handle(
        GetCategoryInput request, 
        CancellationToken cancellationToken
        )
    {
        var category = await _repository.Get(request.Id, cancellationToken);
        return CategoryModelOutput.FromCategory(category);
        
    }
}
