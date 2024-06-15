namespace W.Ind.Core.Helper;

/// <summary>
/// Contains <see langword="static"/> <see langword="string"/> values that represent the names of various JWT token types
/// </summary>
/// <remarks>Use for the '<c>name</c>' and '<c>scheme</c>' params when invoking <c><see langword="await"/> HttpContext.GetTokenAsync(scheme,name)</c> or <c><see langword="await"/> HttpContext.GetTokenAsync(name)</c></remarks>
public static class TokenTypes
{
    /// <summary>
    /// Token name =&gt; "<c>refresh_token</c>"
    /// </summary>
    public static string Refresh => "refresh_token";

    /// <summary>
    /// Token name =&gt; "<c>access_token</c>"
    /// </summary>
    public static string Access => "access_token";

    /// <summary>
    /// Token name =&gt; "<c>id_token</c>"
    /// </summary>
    public static string Id => "id_token";

    /// <summary>
    /// Token name =&gt; "<c>Bearer</c>"
    /// </summary>
    public static string Bearer => "Bearer";

    /// <summary>
    /// Groups together names with a similar prefix ("<c>Identity.</c>")
    /// </summary>
    public static class Identity 
    {
        private static string prefix => "Identity.";

        /// <summary>
        /// Token name =&gt; "<c>Identity.Application</c>"
        /// </summary>
        public static string Application => $"{prefix}.Application";

        /// <summary>
        /// Token name =&gt; "<c>Identity.External</c>"
        /// </summary>
        public static string External => $"{prefix}.External";

        /// <summary>
        /// Groups together token names with a similar prefix ("<c>Identity.TwoFactor</c>")
        /// </summary>
        public static class TwoFactor 
        {
            private static string prefix => $"{Identity.prefix}.TwoFactor";

            /// <summary>
            /// Token name =&gt; "<c>Identity.TwoFactorRememberMe</c>"
            /// </summary>
            public static string RememberMe => $"{prefix}RememberMe";

            /// <summary>
            /// Token name =&gt; "<c>Identity.TwoFactorUserId</c>"
            /// </summary>
            public static string UserId => $"{prefix}UserId";
        }
    }
}
