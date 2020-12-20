using System.Collections.Generic;
using System.Security.Claims;

namespace DatingApp.API.Tests.Helpers
{
    internal class ClaimsPrincipalHelper
    {
        internal static ClaimsPrincipal CreateFakeUser()
        {
            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "FakeUserName"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };

            return new ClaimsPrincipal(
                new ClaimsIdentity(claimList, "TestAuthType"));
        }
    }
}
