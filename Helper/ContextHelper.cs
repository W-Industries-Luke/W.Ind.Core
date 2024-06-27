using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Text.Json;
using W.Ind.Core.Config;

namespace W.Ind.Core.Helper;

/// <summary>
/// This <see langword="static"/> helper class contains useful methods for defining a <see cref="DbContext"/>.
/// </summary>
public static class ContextHelper
{
    public static EntityTypeBuilder<TEntity> ConfigureAudit<TEntity>(this EntityTypeBuilder<TEntity> builder, string tableName = "", TemporalConfig? config = null)
        where TEntity : class, IAuditable, new()
    {
        return builder.ConfigureAudit<TEntity, CoreUser>(tableName, config);
    }

    public static EntityTypeBuilder<TEntity> ConfigureAudit<TEntity, TUser>(this EntityTypeBuilder<TEntity> builder, string tableName = "", TemporalConfig? config = null)
        where TEntity : class, IAuditable<TUser>, new() where TUser : UserBase, new()
    {
        return builder.ConfigureAudit<TEntity, TUser, long>(tableName, config);
    }

    public static EntityTypeBuilder<TEntity> ConfigureAudit<TEntity, TUser, TKey>(this EntityTypeBuilder<TEntity> builder, string tableName = "", TemporalConfig? config = null)
        where TEntity : class, IAuditable<TKey, TUser>, new() where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new()
    {
        new AuditConfiguration<TEntity, TUser, TKey>(tableName, config).Configure(builder);
        return builder;
    }

    public static EntityTypeBuilder<TEntity> FilterDeleted<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class, ISoftDelete
    {
        builder.HasQueryFilter(entity => !entity.IsDeleted);
        return builder;
    }

    public static EntityTypeBuilder<TEntity> BuildIndexes<TEntity>(this EntityTypeBuilder<TEntity> builder, Action<IndexBuilder> configure, params Expression<Func<TEntity, object?>>[] indexExpressions)
        where TEntity : class
    {
        foreach (var expression in indexExpressions) 
        {
            var indexBuilder = builder.HasIndex(expression);
            configure(indexBuilder);
        }

        return builder;
    }

    public static EntityTypeBuilder<TEntity> OneToOne<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TRelated?>> hasOneExpression, Action<ReferenceReferenceBuilder<TEntity, TRelated>>? configure = null, Expression<Func<TRelated, TEntity?>>? withOneExpression = null)
        where TEntity: class where TRelated : class
    {
        if (hasOneExpression == null) { throw new ArgumentNullException(nameof(hasOneExpression)); }
        
        var hasOne = builder.HasOne(hasOneExpression);
        var withOne = withOneExpression == null ? hasOne.WithOne() : hasOne.WithOne(withOneExpression);

        configure?.Invoke(withOne);

        return builder;
    }

    public static EntityTypeBuilder<TEntity> OneToMany<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TRelated?>> hasOneExpression, Action<ReferenceCollectionBuilder<TRelated, TEntity>>? configure = null, Expression<Func<TRelated, IEnumerable<TEntity>?>>? withManyExpression = null)
        where TEntity : class where TRelated : class
    {
        if (hasOneExpression == null) { throw new ArgumentNullException(nameof(hasOneExpression)); }

        var hasOne = builder.HasOne(hasOneExpression);
        var withMany = withManyExpression == null ? hasOne.WithMany() : hasOne.WithMany(withManyExpression);

        configure?.Invoke(withMany);

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ManyToOne<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, IEnumerable<TRelated>?>> hasManyExpression, Action<ReferenceCollectionBuilder<TEntity, TRelated>>? configure = null, Expression<Func<TRelated, TEntity?>>? withOneExpression = null) 
        where TEntity : class where TRelated : class
    {
        if (hasManyExpression == null) { throw new ArgumentNullException(nameof(hasManyExpression)); }

        var hasMany = builder.HasMany(hasManyExpression);
        var withOne = withOneExpression == null ? hasMany.WithOne() : hasMany.WithOne(withOneExpression);

        configure?.Invoke(withOne);

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ManyToMany<TEntity, TRelated>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, IEnumerable<TRelated>?>> hasManyExpression, Action<CollectionCollectionBuilder<TRelated, TEntity>>? configure = null, Expression<Func<TRelated, IEnumerable<TEntity>?>>? withManyExpression = null)
        where TEntity : class where TRelated : class
    {
        if (hasManyExpression == null) { throw new ArgumentNullException(nameof(hasManyExpression)); }

        var hasMany = builder.HasMany(hasManyExpression);
        var withMany = withManyExpression == null ? hasMany.WithMany() : hasMany.WithMany(withManyExpression);

        configure?.Invoke(withMany);

        return builder;
    }

    public static EntityTypeBuilder<TEntity> CompositeKey<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, object?>> keysExpression, Action<KeyBuilder>? configure = null)
        where TEntity : class, IJoinTable
    {
        if (keysExpression == null) { throw new ArgumentNullException(nameof(keysExpression)); }

        var keyBuilder = builder.HasKey(keysExpression);
        configure?.Invoke(keyBuilder);

        return builder;
    }

    /// <summary>
    /// A <see langword="static"/> method that configures a temporal table based off generic type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <remarks>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="void"/> onModelCreating(<see cref="ModelBuilder"/> builder) {
    ///     builder.<see cref="ModelBuilder.Entity{TEntity}()">Entity</see>&lt;<typeparamref name="TEntity"/>&gt;()
    ///         .<see cref="RelationalEntityTypeBuilderExtensions.ToTable{TEntity}(EntityTypeBuilder{TEntity}, Action{TableBuilder{TEntity}})">ToTable</see>(<see cref="ContextHelper"/>.<see cref="BuildTemporal{TEntity}(TableBuilder{TEntity})">BuildTemporal</see>)
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <typeparam name="TEntity">The entity type to build the Temporal table from.</typeparam>
    /// <param name="builder">TableBuilder instance exposed through the EF Core ModelBuilder</param>
    public static void BuildTemporal<TEntity>(TableBuilder<TEntity> builder) where TEntity : class, new()
    {
        builder.IsTemporal(temporal => 
        {
            string typeName = typeof(TEntity).Name;
            temporal.HasPeriodStart("SysStartTime");
            temporal.HasPeriodEnd("SysEndTime");
            temporal.UseHistoryTable($"{typeName}-History");
        });
    }

    /// <summary>
    /// A <see langword="static"/> method that configures a temporal table based off generic type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The resulting temporal table will have a custom name defined with <paramref name="config"/>.<see cref="TemporalConfig.HistoryTableName">HistoryTableName</see>
    /// </para>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="void"/> <see cref="DbContext.OnModelCreating(ModelBuilder)">onModelCreating</see>(<see cref="ModelBuilder"/> builder) {
    ///     builder<see cref="ModelBuilder.Entity{TEntity}()">.Entity</see>&lt;<typeparamref name="TEntity"/>&gt;()
    ///         .<see cref="RelationalEntityTypeBuilderExtensions.ToTable{TEntity}(EntityTypeBuilder{TEntity}, Action{TableBuilder{TEntity}})">ToTable</see>(<see cref="TableBuilder"/> builder =>
    ///             <see cref="ContextHelper"/>.<see cref="BuildTemporal{TEntity}(TableBuilder{TEntity}, TemporalConfig)">BuildTemporal</see>(builder, "CustomHistoryTableName");
    ///         );
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <typeparam name="TEntity">Any entity with a corresponding SQL table.</typeparam>
    /// <param name="builder"><see cref="TableBuilder"/> instance, accessable through the <see cref="RelationalEntityTypeBuilderExtensions.ToTable{TEntity}(EntityTypeBuilder{TEntity}, Action{TableBuilder{TEntity}})">ToTable</see>() method in <see cref="DbContext.OnModelCreating(ModelBuilder)"/> "OnModelCreating"</param>
    /// <param name="config">Configuration options to customize this temporal table</param>
    public static void BuildTemporal<TEntity>(TableBuilder<TEntity> builder, TemporalConfig config) where TEntity: class 
    {
        config.HistoryTableName = String.IsNullOrWhiteSpace(config.HistoryTableName) ? typeof(TEntity).Name : config.HistoryTableName;
        config.PeriodStartName = String.IsNullOrWhiteSpace(config.PeriodStartName) ? "SysStartTime" : config.PeriodStartName;
        config.PeriodEndName = String.IsNullOrWhiteSpace(config.PeriodEndName) ? "SysEndTime" : config.PeriodEndName;
        builder.IsTemporal(temporal => 
        {
            temporal.HasPeriodStart(config.PeriodStartName);
            temporal.HasPeriodEnd(config.PeriodEndName);
            temporal.UseHistoryTable($"{config.HistoryTableName}-History");
        });
    }

    /// <summary>
    /// A <see langword="static"/> method that handles when an Entity (that's derived from <see cref="ISoftDelete"/>) is deleted.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Resets <see cref="EntityState"/> to <see cref="EntityState.Modified"/> and sets the <see cref="ISoftDelete.IsDeleted"/> flag to <see langword="true"/>.
    /// </para>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="int"/> <see cref="DbContext.SaveChanges()">SaveChanges</see>() {
    ///     <see cref="ChangeTracker"/>.<see cref="ChangeTracker.Entries{ISoftDelete}()">Entries</see>&lt;<see cref="ISoftDelete"/>&gt;()
    ///         .<see cref="HandleSoftDelete(IEnumerable{EntityEntry{ISoftDelete}})">HandleSoftDelete</see>();
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="entries"><see cref="ChangeTracker.Entries{ISoftDelete}()" /></param>
    public static void HandleSoftDelete(this IEnumerable<EntityEntry<ISoftDelete>> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
            }
        }
    }

    public static void HandleAudit<TAuditable>(this IEnumerable<EntityEntry<TAuditable>> entries, long currentUser)
        where TAuditable : class, IAuditable<long, CoreUser>
    {
        entries.HandleAudit<TAuditable, CoreUser>(currentUser);
    }

    public static void HandleAudit<TAuditable, TUser>(this IEnumerable<EntityEntry<TAuditable>> entries, long currentUser)
        where TUser : UserBase where TAuditable : class, IAuditable<long, TUser>
    {
        entries.HandleAudit<TAuditable, long, TUser>(currentUser);
    }

    /// <summary>
    /// A <see langword="static"/> method used to update the audit log fields provided by <see cref="IAuditable"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Call <c><see cref="UserService{TUser,TKey}.GetCurrent"/></c> to retrieve the current user's ID.
    /// </para>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="int"/> <see cref="DbContext.SaveChanges()">SaveChanges</see>() {
    ///     <see cref="ChangeTracker"/>.<see cref="ChangeTracker.Entries{IAuditable}()">Entries</see>&lt;<see cref="IAuditable"/>&gt;()
    ///         .<see cref="HandleAudit(IEnumerable{EntityEntry{IAuditable}}, long)">HandleAudit</see>(<see cref="UserService{TUser,TKey}.GetCurrent"/>);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="entries"><see cref="ChangeTracker.Entries{IAuditable}()"/></param>
    /// <param name="currentUser"><see cref="UserService{TUser,TKey}.GetCurrent"/></param>
    public static void HandleAudit<TAuditable, TKey, TUser>(this IEnumerable<EntityEntry<TAuditable>> entries, TKey currentUser)
        where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey> where TAuditable : class, IAuditable<TKey, TUser>
    {
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Modified:
                    entry.Entity.ModifiedOn = now;
                    entry.Property<TKey>("ModifiedById").CurrentValue = currentUser;
                    break;
                case EntityState.Added:
                    entry.Entity.CreatedOn = now;
                    entry.Entity.CreatedById = currentUser;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// A <see langword="static"/> method used to parse an entity's <see cref="DateTime"/> properties to UTC.
    /// </summary>
    /// <remarks>
    /// <para>Useful when <see langword="override">overriding</see> <see cref="DbContext.SaveChanges()"/></para>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="int"/> <see cref="DbContext.SaveChanges()">SaveChanges</see>() {
    ///     <see cref="ChangeTracker"/>.<see cref="ChangeTracker.Entries{IDate}()">Entries</see>&lt;<see cref="IDate"/>&gt;.<see cref="ParseUtcDates(IEnumerable{EntityEntry{IDate}})">ParseUtcDates</see>();
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="entries">"<see cref="ChangeTracker.Entries{IDate}()"/></param>
    public static void ParseUtcDates(this IEnumerable<EntityEntry<IDate>> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                UtcDateHelper.SetUtcIfDataExists<IStartDate>(entry.Entity);
                UtcDateHelper.SetUtcIfDataExists<IEndDate>(entry.Entity);
                UtcDateHelper.SetUtcIfDataExists<IEnterredDate>(entry.Entity);
            }
        }
    }

    /// <summary>
    /// DEPRECIATED: A <see langword="static"/> method that reads, dererializes, and returns data from JSON files.
    /// </summary>
    /// <remarks>
    /// <para>The file path is relative to the startup project directory while referenced inside <see cref="DbContext.OnModelCreating(ModelBuilder)"/>.</para>
    /// <para>Useful for seeding data.</para>
    /// <example>
    /// Usage:
    /// <code>
    /// <see langword="protected"/> <see langword="override"/> <see langword="void"/> <see cref="DbContext.OnModelCreating(ModelBuilder)">OnModelCreating</see>(<see cref="ModelBuilder"/> builder) {
    ///     builder.<see cref="ModelBuilder.Entity{TEntity}()">Entity</see>&lt;<typeparamref name="TEntity"/>&gt;()
    ///         .<see cref="EntityTypeBuilder{TEntity}.HasData(IEnumerable{object})">HasData</see>(<see cref="ContextHelper"/>.<see cref="GetFromJsonFile{TEntity}(string)">GetFromJsonFile</see>(<c>"seed/data.json"</c>));
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    /// <typeparam name="TEntity">The resulting data type of the deserialized JSON</typeparam>
    /// <param name="filePath">Path is relative to startup project when called in your DbContext's "OnModelCreating" method</param>
    /// <returns>The JSON data deserialized as an instance of <typeparamref name="TEntity"/></returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is empty or contains any restricted characters</exception>
    /// <exception cref="NullReferenceException">Thrown when the deserialized <typeparamref name="TEntity"/> is <see langword="null"/></exception>
    /// <exception cref="PathTooLongException">Thrown when <paramref name="filePath"/> is longer than 260 characters</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when <paramref name="filePath"/> points to any non-existent directory</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when method caller doesn't have read permissions for the specified file</exception>
    /// <exception cref="NotSupportedException"></exception>
    [Obsolete("ContextHelper.GetFromJsonFile is Depreciated. Use SeedFromJson method instead.")]
    public static TEntity GetFromJsonFile<TEntity>(string filePath) where TEntity : class
    {
        if (String.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException("File Path Empty"); }
        var jsonData = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<TEntity>(jsonData)!;
    }

    public static EntityTypeBuilder<TEntity> SeedFromJson<TEntity>(this EntityTypeBuilder<TEntity> builder, string jsonRef, bool isFile = true) where TEntity : class 
    {
        if (String.IsNullOrWhiteSpace(jsonRef)) { throw new ArgumentNullException($"JSON Seeding Error - {typeof(TEntity).Name}: The JSON string reference was null"); }

        string jsonString = isFile ? File.ReadAllText(jsonRef) : jsonRef;
        List<TEntity>? deserialized = JsonSerializer.Deserialize<List<TEntity>>(jsonString);

        builder.AddSeedData(deserialized);

        return builder;
    }

    public static void AddSeedData<TEntity, TData>(this EntityTypeBuilder<TEntity> builder, TData? data) 
        where TEntity : class where TData : List<TEntity>
    {
        if (data == null) { throw new InvalidCastException($"JSON Seeding Error - {typeof(TEntity).Name}: Deserialized seed data was null"); }

        builder.HasData(data);
    }

    /// <summary>
    /// Used to parse an entity's Primary Key from a <see langword="string"/> to an instance of <typeparamref name="TKey"/>
    /// </summary>
    /// <remarks>
    /// Useful for parsing keys retrieved from <see cref="Microsoft.AspNetCore.Http.HttpContext"/>
    /// </remarks>
    /// <typeparam name="TKey">The data type this key should be parsed into</typeparam>
    /// <param name="key"><see langword="string"/> instance of a Primary Key value</param>
    /// <returns></returns>
    /// <exception cref="FormatException">Thrown when the Primary Key <see langword="type"/> is neither Integer based, nor <see langword="string"/>/<see cref="Guid"/></exception>
    public static TKey ParsePrimaryKey<TKey>(string key) where TKey : struct, IEquatable<TKey> 
    {
        var type = typeof(TKey);
        if (GenericTypeHelper.IsIntegerType<TKey>())
        {
            if (long.TryParse(key, out var longKey))
            {
                return (TKey)(object)longKey;
            }
        }
        else if (type == typeof(string)) 
        {
            return (TKey)(object)key;
        }
        else if (type == typeof(Guid))
        {
            if (Guid.TryParse(key, out var guidKey))
            {
                return (TKey)(object)guidKey;
            }
        }

        throw new FormatException($"Unable to convert '{key}' to {type}.");
    }
}
