using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Notes.Identity
{
    public class Configuration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("NotesWebAPI", "Web API")
            };
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        public static IEnumerable<ApiResource> ApiResource =>
            new List<ApiResource>
            {
                new ApiResource("NotesWebApi", "Wev API", new[]
                {
                    JwtClaimTypes.Name
                })
                {
                    Scopes = {"NotesWebAPI"}
                }
            };

        public static IEnumerable<Client> Clients =>
            new List<Client> // which clients can use app
            {
                new Client
                {
                    ClientId = "notes-web-api", // grants that id of user is same 
                    ClientName = "Notes Web",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false, // we do not use secret of client SSH 200
                    RequirePkce = true,
                    RedirectUris =
                    {
                        "http://.../signin-oidc"
                    },
                    AllowedCorsOrigins =
                    {
                        "http://..."
                    },
                    PostLogoutRedirectUris = // where client goes after sighn out
                    {
                        "http://.../signout-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "NotesWebAPI"
                    },
                    AllowAccessTokensViaBrowser = true
                }

            };
    }
}
