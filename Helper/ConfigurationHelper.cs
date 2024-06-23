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
    public static IServiceCollection ConfigureWicServices(this IServiceCollection services, JwtConfig config)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        
        return services.ConfigureWicServices<User>(config);
    }

    public static IServiceCollection ConfigureWicServices<TUser>(this IServiceCollection services, JwtConfig config)
        where TUser : UserBase<long>, new()
    {
        services.AddScoped<IJwtService<TUser>, JwtService<TUser>>();
        services.AddScoped<IUserService<TUser>, UserService<TUser>>();
        
        return services.ConfigureWicServices<long, TUser>(config);
    }

    public static IServiceCollection ConfigureWicServices<TKey, TUser>(this IServiceCollection services, JwtConfig config)
        where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new()
    {
        return services.ConfigureWicServices<TKey, TUser, JwtConfig>(config);
    }

    /// <summary>
    /// A <see langword="static"/> extension method used to configure <see cref="Core"/> services in one method
    /// </summary>
    /// <typeparam name="TConfig"><see cref="JwtConfig"/> or any type that inherits from it</typeparam>
    /// <typeparam name="TUser">Your User entity type (must inherit from <see cref="UserBase{TKey}"/>)</typeparam>
    /// <typeparam name="TKey">The data type of the User entity's <c>Primary Key</c></typeparam>
    /// <param name="services"><c>builder.Services</c></param>
    /// <param name="config">An instance of <typeparamref name="TConfig"/> mapped from your Configuration file</param>
    /// <returns>The same <see cref="IServiceCollection"/> that called this method</returns>
    public static IServiceCollection ConfigureWicServices<TKey, TUser, TConfig>(this IServiceCollection services, TConfig config) 
        where TConfig : JwtConfig where TKey : struct, IEquatable<TKey> where TUser : UserBase<TKey>, new()
    {
        services.AddHttpContextAccessor();
        services.AddSingleton(config);
        services.AddSingleton<IJwtInvalidator, JwtInvalidator>();

        services.AddScoped<IJwtService<TKey, TUser>, JwtService<TKey, TUser, TConfig>>();
        services.AddScoped<IUserService<TKey, TUser>, UserService<TKey, TUser>>();
        
        return services;
    }
}
