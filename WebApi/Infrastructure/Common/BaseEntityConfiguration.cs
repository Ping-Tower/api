using Domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Common;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.CreatedAt)
        .IsRequired(false);

        builder.Property(w => w.CreatedBy)
        .IsRequired(false);

        builder.Property(w => w.ModifiedAt)
        .IsRequired(false);

        builder.Property(w => w.ModifiedBy)
        .IsRequired(false);
    }
}