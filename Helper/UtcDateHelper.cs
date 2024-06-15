namespace W.Ind.Core.Helper;

/// <summary>
/// This <see langword="static"/> helper class is used to identify and parse an entity's <see cref="DateTime"/> properties to UTC
/// </summary>
/// <remarks>
/// Use with <see cref="IDate"/> and any of it's deriving <see langword="interface"/> to specify which <see cref="DateTime"/> properties to parse
/// </remarks>
public static class UtcDateHelper
{
    /// <summary>
    /// Uses any <see langword="interface"/> derived from <see cref="IDate"/> to parse it's <see cref="DateTime"/> properties to UTC
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cast your entity into an <see cref="IDate"/> with the <see langword="as"/> keyword if it derives from any <typeparamref name="TType"/> <see langword="interface"/>
    /// </para>
    /// <para>
    /// See <see cref="ContextHelper.ParseUtcDates"/> implementation for usage
    /// </para>
    /// <para>
    /// Loops through all <see cref="DateTime"/> properties defined in <typeparamref name="TType"/>
    /// </para>
    /// </remarks>
    /// <typeparam name="TType">Any <see langword="interface"/> type that derives from <see cref="IDate"/>.</typeparam>
    /// <param name="entity">Can cast any entity (using the 'as' keyword) that implements "IDate". Or an interface derived from "IDate".</param>
    public static void SetUtcIfDataExists<TType>(IDate entity) where TType : class, IDate
    {
        Type type = typeof(TType);
        if (type.IsAssignableFrom(entity.GetType()))
        {
            TType? date = entity as TType;
            if (date != null)
            {
                IEnumerable<System.Reflection.PropertyInfo> properties = type.GetProperties().Where(f => f.PropertyType == typeof(DateTime));
                foreach (var property in properties) 
                {
                    var value = property.GetValue(date) as DateTime?;
                    if (value.HasValue)
                    {
                        property.SetValue(date, value.Value.ToUniversalTime());
                    }
                }
            }
        }
    }

}