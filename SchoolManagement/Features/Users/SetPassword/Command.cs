using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Model.Domain;

namespace SchoolManagement.Features.Users.SetPassword
{
    public class Command : IRequest<IdentityResult>
    {
        public string Id { get; set; }
        public string NewPassword { get; set; }
    }

    public class Handler : IRequestHandler<Command, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public Handler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> Handle(Command request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByIdAsync(request.Id);

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!identityResult.Succeeded)
            {
                string errorMessage = identityResult.Errors
                    .Select(x=>x.Description)
                    .Aggregate((x,y)=>$"{x}; {y}");
                throw new ChangePasswordException(errorMessage);
            }

            return identityResult;
        }
    }

    public class ChangePasswordException : Exception
    {
        public ChangePasswordException(string message):base(message)
        {
            
        }
    }
}
