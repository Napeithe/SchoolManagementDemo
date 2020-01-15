using System.Collections.Generic;
using System.Security.Claims;
using Model.Auth;

namespace Builders
{
    public class ClaimsPrincipalBuilder : Builder<ClaimsPrincipalBuilder, ClaimsPrincipal>
    {
        private readonly List<Claim> _permissionClaims = new List<Claim>();

        public ClaimsPrincipalBuilder AddPermissionClaim(string claimName)
        {
            Claim claim = new Claim(CustomClaimTypes.Permission, claimName);

            _permissionClaims.Add(claim);
            return this;
        }

        public override ClaimsPrincipal Build()
        {
            State.AddIdentity(new ClaimsIdentity(_permissionClaims));
            return base.Build();
        }
    }
}
