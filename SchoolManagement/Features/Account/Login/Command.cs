using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;
using Model.Domain.Interface;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Account.Login
{
    public class Command : IRequest<DataResult<ClaimsPrincipal>>, IUtcOffset
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult<ClaimsPrincipal>>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SchoolManagementContext _context;

        public Handler(SignInManager<User> signInManager, SchoolManagementContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<DataResult<ClaimsPrincipal>> Handle(Command request, CancellationToken cancellationToken)
        {
            Model.Domain.User user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return DataResult<ClaimsPrincipal>.Error(PolishReadableMessage.Auth.WrongLoginOrPassword);
            }

            SignInResult resultPassword =
                await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (resultPassword.Succeeded)
            {
                if (user.EmailConfirmed)
                {
                    user.UtcOffsetInMinutes = request.UtcOffsetInMinutes;
                    _context.Update(user);
                    await _context.SaveChangesAsync(cancellationToken);

                    await _signInManager.SignInAsync(user, request.RememberMe);
                    ClaimsPrincipal claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                    return DataResult<ClaimsPrincipal>.Success(claimsPrincipal);
                }

                return DataResult<ClaimsPrincipal>.Error(PolishReadableMessage.Auth.AccountNotActive);
            }

            return DataResult<ClaimsPrincipal>.Error(PolishReadableMessage.Auth.WrongLoginOrPassword);
        }
    } 
}


