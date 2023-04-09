﻿using FC.Codeflix.Catalog.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Config;
internal class CategoryConfig
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Name)
            .HasMaxLength(255);
        builder.Property(category => category.Description)
            .HasMaxLength(10000);
    }
}
