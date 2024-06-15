using Microsoft.Extensions.DependencyInjection;
using W.Ind.Core.Config;

namespace W.Ind.Core.Helper;

/// <summary>
/// A <see langword="static"/> helper <see langword="class"/> used to define <see cref="IServiceCollection"/> extension methods
/// </summary>
/// <remarks>
/// Quickly configure all W.Ind.Core services and their dependencies
/// </remarks>
public static class ConfigurationHelper
{
    /// <summary>
    /// A <see langword="static"/> extension method used to configure <see cref="Core"/> services in one method
    /// </summary>
    /// <typeparam name="TConfig"><see cref="JwtConfig"/> or any type that inherits from it</typeparam>
    /// <typeparam name="TUser">Your User entity type (must inherit from <see cref="UserBase{TKey}"/>)</typeparam>
    /// <typeparam name="TKey">The data type of the User entity's <c>Primary Key</c></typeparam>
    /// <param name="services"><c>builder.Services</c></param>
    /// <param name="config">An instance of <typeparamref name="TConfig"/> mapped from your Configuration file</param>
    /// <returns>The same <see cref="IServiceCollection"/> that called this method</returns>
    public static IServiceCollection ConfigureWicServices<TConfig, TUser, TKey>(this IServiceCollection services, TConfig config) 
        where TConfig : JwtConfig where TUser : UserBase<TKey>, new() where TKey : IEquatable<TKey>
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(config);
        services.AddSingleton<IJwtInvalidator, JwtInvalidator>();
        services.AddScoped<IJwtService<TUser, TKey>, JwtService<TUser, TKey, TConfig>>();
        services.AddScoped<IUserService<TUser, TKey>, UserService<TUser, TKey>>();
        return services;
    }
}
