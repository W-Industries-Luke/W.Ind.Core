namespace W.Ind.Core.Config;

/// <summary>
/// POCO <see langword="class"/> designed to map from the JWT section of a Configuration file
/// </summary>
/// <remarks>
/// Extend this <see langword="class"/> to map more options from your Configuration file's JWT section
/// </remarks>
public class JwtConfig
{
    /// <summary>
    /// JWT Key Value
    /// </summary>
    public string SecretKey { get; set; } = String.Empty;

    /// <summary>
    /// JWT Issuer Value
    /// </summary>
    public string Issuer { get; set; } = String.Empty;

    /// <summary>
    /// JWT Audience Value
    /// </summary>
    public string Audience { get; set; } = String.Empty;

    /// <summary>
    /// JWT Validate Issuer value
    /// </summary>
    public bool ValidateIssuer { get; set; } = false;

    /// <summary>
    /// JWT Validate Audience value
    /// </summary>
    public bool ValidateAudience { get; set; } = false;

    /// <summary>
    /// JWT Validate Issuer Signing Key value
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; } = false;
}
