using Microsoft.EntityFrameworkCore.Metadata.Builders;
using W.Ind.Core.Config;

namespace W.Ind.Core.Entity;

public class AuditConfiguration<TEntity> 
    : AuditConfiguration<TEntity, User> 
    where TEntity : class, IAuditable<long, User>, new();

public class AuditConfiguration<TEntity, TUser> 
    : AuditConfiguration<TEntity, TUser, long> 
    where TEntity : class, IAuditable<long, TUser>, new() where TUser : UserBase<long>;

public class AuditConfiguration<TEntity, TUser, TUserKey> 
    : IEntityTypeConfiguration<TEntity> 
    where TEntity : class, IAuditable<TUserKey, TUser>, new() where TUserKey : struct, IEquatable<TUserKey> where TUser : UserBase<TUserKey>
{
    protected readonly string _tableName;
    protected readonly TemporalConfig? _config;
    public AuditConfiguration(string tableName = "", TemporalConfig? config = null) 
    {
        _tableName = tableName;
        _config = config;
    }

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasOne(entity => entity.CreatedBy).WithMany()
            .HasForeignKey(entity => entity.CreatedById).IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        // TODO: Figure out Modified ID Shadow Property or another way to get nullable modified id on here
        builder.HasOne(entity => entity.ModifiedBy).WithMany()
            .HasForeignKey("ModifiedById")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(entity => entity.Timestamp).IsRowVersion();

        bool isNameEmpty = String.IsNullOrWhiteSpace(_tableName);

        if (isNameEmpty && _config == null)
        {
            builder.ToTable(ContextHelper.BuildTemporal);
        }
        else if (!isNameEmpty && _config != null)
        {
            builder.ToTable(_tableName, tableBuilder => ContextHelper.BuildTemporal(tableBuilder, _config!));
        }
        else if (!isNameEmpty)
        {
            builder.ToTable(_tableName, ContextHelper.BuildTemporal);
        }
        else
        {
            builder.ToTable(tableBuilder => ContextHelper.BuildTemporal(tableBuilder, _config!));
        }
    }
}