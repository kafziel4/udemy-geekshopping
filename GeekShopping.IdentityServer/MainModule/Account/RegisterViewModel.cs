using IdentityServerHost.Quickstart.UI;

namespace GeekShopping.IdentityServer.MainModule.Account
{
    public class RegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public string? RoleName { get; set; }
        public bool AllowRememberLogin { get; set; } = true;
        public bool EnableLocalLogin { get; set; } = true;
        public IEnumerable<ExternalProvider> ExternalProviders { get; set; } =
            Enumerable.Empty<ExternalProvider>();
        public IEnumerable<ExternalProvider> VisibleExternalProviders =>
            ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
        public bool IsExternalLoginOnly => !EnableLocalLogin && ExternalProviders.Count() == 1;
        public string? ExternalLoginScheme =>
            IsExternalLoginOnly ? ExternalProviders.SingleOrDefault()?.AuthenticationScheme : null;
    }
}
