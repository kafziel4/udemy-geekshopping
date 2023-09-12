using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DbInitializer(UserManager<ApplicationUser> user, RoleManager<IdentityRole> role)
        {
            _user = user;
            _role = role;
        }

        public async Task Initialize()
        {
            if (await _role.FindByNameAsync(IdentityConfiguration.Admin) is not null)
                return;

            await _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin));
            await _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client));

            var admin = new ApplicationUser
            {
                UserName = "some-admin",
                Email = "some-admin@mail.com.br",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 12345-6789",
                FirstName = "Some",
                LastName = "Admin"
            };

            await _user.CreateAsync(admin, "Numsey123$");
            await _user.AddToRoleAsync(admin, IdentityConfiguration.Admin);
            await _user.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
            });

            var client = new ApplicationUser
            {
                UserName = "some-client",
                Email = "some-client@mail.com.br",
                EmailConfirmed = true,
                PhoneNumber = "+55 (11) 12345-6789",
                FirstName = "Some",
                LastName = "Client"
            };

            await _user.CreateAsync(client, "Numsey123$");
            await _user.AddToRoleAsync(client, IdentityConfiguration.Client);
            await _user.AddClaimsAsync(client, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
                new Claim(JwtClaimTypes.GivenName, client.FirstName),
                new Claim(JwtClaimTypes.FamilyName, client.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
            });
        }
    }
}
