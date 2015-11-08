using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Tdlr.DAL;

namespace Tdlr.Utils
{
    class GraphHelper
    {
        public static async Task<string> AcquireTokenAsUser()
        {
            ClientCredential cred = new ClientCredential(ConfigHelper.ClientId, ConfigHelper.AppKey);
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            string userObjectId = ClaimsPrincipal.Current.FindFirst(Globals.ObjectIdClaimType).Value;
            AuthenticationContext authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, tenantId), new TokenDbCache(userObjectId));
            AuthenticationResult result = authContext.AcquireTokenSilent(ConfigHelper.GraphResourceId, cred, new UserIdentifier(userObjectId, UserIdentifierType.UniqueId));
            return result.AccessToken;
        }

        public static async Task<string> AcquireTokenAsApp()
        {
            ClientCredential cred = new ClientCredential(ConfigHelper.ClientId, ConfigHelper.AppKey);
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;
            AuthenticationContext authContext = new AuthenticationContext(String.Format(CultureInfo.InvariantCulture, ConfigHelper.AadInstance, tenantId));
            AuthenticationResult result = authContext.AcquireToken(ConfigHelper.GraphResourceId, cred);
            return result.AccessToken;
        }
    }
}
