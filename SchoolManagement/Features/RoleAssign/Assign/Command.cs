using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.RoleAssign.Assign
{
    public class Command : IRequest<bool>
    {
        public string UserId { get; set; }
        public string RoleName { get; set; }
        public string ReturnUrl { get; set; } = "";
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
            User user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                throw new AssignToRoleException(PolishReadableMessage.Assign.UserNotFound);
            }

            bool isInRoleAsync = await _userManager.IsInRoleAsync(user, request.RoleName);
            if (isInRoleAsync)
            {
                throw new AssignToRoleException(PolishReadableMessage.Assign.UserIsAssignedToRoleAlready);
            }

            await _userManager.AddToRoleAsync(user, request.RoleName);
            return true;
        }
    }

    public class AssignToRoleException : Exception
    {
        public AssignToRoleException(string message):base(message)
        {
            
        }
    }
}
