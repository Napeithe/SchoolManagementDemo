using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Model.Auth;
using Model.Domain;

namespace Model.DataSeeders
{
    public class RoleDataSeeder : DataSeederCore
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleDataSeeder(SchoolManagementContext context, RoleManager<Role> roleManager) : base(context)
        {
            _roleManager = roleManager;
        }

        public override bool DataExists()
        {
            return false;
        }

        public override async Task SeedData()
        {
            var seedRoles = new List<SeedRole>
            {
                new SeedRole
                {
                    Role = new Role(Roles.SuperAdmin, Roles.Descriptions[Roles.SuperAdmin]),
                    Claims = RoleClaims.SuperAdminClaims
                },
                new SeedRole
                {
                    Role = new Role(Roles.Admin, Roles.Descriptions[Roles.Admin]),
                    Claims = RoleClaims.AdminClaims,
                },
                new SeedRole
                {
                    Role = new Role(Roles.Anchor, Roles.Descriptions[Roles.Anchor]),
                    Claims = RoleClaims.AnchorClaims
                },
                new SeedRole
                {
                    Role = new Role(Roles.Participant, Roles.Descriptions[Roles.Participant]),
                    Claims = RoleClaims.ParticipantClaims
                },

            };

            foreach (SeedRole seedRole in seedRoles)
            {
                bool roleExistsAsync = await _roleManager.RoleExistsAsync(seedRole.Role.Name);
                if (!roleExistsAsync)
                {
                    await _roleManager.CreateAsync(seedRole.Role);

                    var role = await _roleManager.FindByNameAsync(seedRole.Role.Name);
                    foreach (Claim claim in seedRole.Claims)
                    {
                        await _roleManager.AddClaimAsync(role, claim);
                    }
                }
                else
                {
                    var role = await _roleManager.FindByNameAsync(seedRole.Role.Name);
                    var claims = await _roleManager.GetClaimsAsync(role);

                    foreach (var claim in claims)
                    {
                        await _roleManager.RemoveClaimAsync(role, claim);
                    }

                    foreach (var claim in seedRole.Claims)
                    {
                        await _roleManager.AddClaimAsync(role, claim);
                    }
                }
            }
        }
    }

    public class SeedRole
    {
        public Role Role { get; set; }
        public List<Claim> Claims { get; set; } = new List<Claim>();
    }
}
