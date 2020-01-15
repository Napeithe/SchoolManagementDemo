using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Account.ConfirmEmail
{
    public class Command : IRequest<DataResult>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult>
    {
        private readonly UserManager<User> _userManager;

        public Handler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return DataResult.Error(PolishReadableMessage.Account.CannotActivateEmailAccountNotExist);
            }

            if (user.EmailConfirmed)
            {
                return DataResult.Error(PolishReadableMessage.Account.UserHasActivatedAccountAlready);
            }

            string decodedToken = HttpUtility.HtmlDecode(request.Token);

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
               return DataResult.Error(result.Errors.FirstOrDefault()?.Description);
            }

            return DataResult.Success();
        }
    }
}
