using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Genre
{
    public Genre(
        string name, 
        bool isActive = true
    )
    {
        Name = name;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
    public void Update(string name)
    {
        Name = name;
        Validate();
    }

    public void Validate()
        => DomainValidation
            .NotNullOrEmpty(Name, nameof(Name));
    
}
