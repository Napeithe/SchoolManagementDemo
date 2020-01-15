using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Auth;
using Model.Domain;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Features.RoleAssign.Unassign
{
    public class Command : IRequest<bool>
    {
        public string RoleName { get; set; }
        public string UserId { get; set; }
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly UserManager<User> _userManager;

        public Handler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            bool hasPermission = VerifyClaims(request.RoleName, request.ClaimsPrincipal);
            if (!hasPermission)
            {
                throw new UnassignRoleException(PolishReadableMessage.Assign.DontHavePermissionForUnassignThisRole);
            }

            User user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
            {
                throw new UnassignRoleException(PolishReadableMessage.UserNotFound);
            }

            await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            return true;
        }

        private bool VerifyClaims(string roleName, ClaimsPrincipal claims)
        {
            switch (roleName)
            {
                case Roles.Anchor:
                    return claims.HasPermissionClaim(Permissions.Anchors.Remove);
                case Roles.Participant:
                    return claims.HasPermissionClaim(Permissions.Participants.Remove);
                default:
                    return false;
            }
        }
    }

    public class UnassignRoleException : Exception
    {
        public UnassignRoleException(string message):base(message)
        {
            
        }
    }
}
