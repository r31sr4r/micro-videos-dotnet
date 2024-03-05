﻿using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Entity;
public class Category : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }


    public Category(string name, string description, bool isActive = true)
        : base()
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = DateTime.Now;

        Validate();
    }

    public void Activate()
    {
        IsActive = true;
        Validate();
    }

    public void Deactivate()
    {
        IsActive = false; Validate();
        Validate();
    }

    public void Update(string name, string? description = null)
    {
        Name = name; 
        Description = description ?? Description;
        Validate();
    }

    private void Validate()
    {
        DomainValidation.NotNullOrEmpty(Name, nameof(Name));
        DomainValidation.MinLenght(Name, 3, nameof(Name));
        DomainValidation.MaxLenght(Name, 255, nameof(Name));
        DomainValidation.NotNull(Description, nameof(Description));
        DomainValidation.MaxLenght(Description, 10000, nameof(Description));
    }

}
