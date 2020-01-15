using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Model.Domain;
using Model.Domain.Interface;
using SchoolManagement.Features.GroupClass.Add;

namespace SchoolManagement.Infrastructure.Identity
{
    public static class CustomClaims
    {
        public static string UtcOffset => "UtcOffset";
    }

    public class CustomClaimsPrincipalFactory<TUser> : UserClaimsPrincipalFactory<TUser>
    where TUser : class, IUtcOffset
    {
        public CustomClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(CustomClaims.UtcOffset, user.UtcOffsetInMinutes.ToString()));
            return identity;
        }
    }

    public class CustomUserClaimsPrincipalFactory<TUser, TRole> : CustomClaimsPrincipalFactory<TUser>
        where TUser : class, IUtcOffset
        where TRole : class
    {
        public CustomUserClaimsPrincipalFactory(UserManager<TUser> userManager, RoleManager<TRole> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, options)
        {
            if (roleManager == null)
            {
                throw new ArgumentNullException(nameof(roleManager));
            }
            RoleManager = roleManager;
        }

        public RoleManager<TRole> RoleManager { get; private set; }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            var id = await base.GenerateClaimsAsync(user);
            if (UserManager.SupportsUserRole)
            {
                var roles = await UserManager.GetRolesAsync(user);
                foreach (var roleName in roles)
                {
                    id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                    if (RoleManager.SupportsRoleClaims)
                    {
                        var role = await RoleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            id.AddClaims(await RoleManager.GetClaimsAsync(role));
                        }
                    }
                }
            }
            return id;
        }
    }
}
