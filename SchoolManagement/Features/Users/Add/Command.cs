using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Auth;
using Model.Domain;
using SchoolManagement.Features.Users.Edit;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Features.Users.Add
{

    public class Command : IRequest<bool>, IUpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> RolesId { get; set; } = new List<string>();
        public bool IsEmailActivated { get; set; }
        public string Id { get; set; } 
        public string RoleName { get; set; }
        public string UrlAddress { get; set; }
        public ClaimsPrincipal User { get; set; }

        public bool ShouldSendEmail { get; set; }

        public User GetUser()
        {
            return new User
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                UserName = Email
            };
        }
    }

    public class Handler : IRequestHandler<Command, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly SchoolManagementContext _context;
        private readonly IEmailSender _emailSender;

        public Handler(UserManager<User> userManager, SchoolManagementContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }
        private bool CheckPermission(List<string> roleName, ClaimsPrincipal user)
        {
            foreach (string role in roleName)
            {
                bool checkPermission = CheckPermission(role, user);
                if (!checkPermission)
                {
                   throw new UserAddException(string.Format(PolishReadableMessage.User.NotPermissionToRole, Roles.Descriptions[role]));
                }
            }

            return true;
        }

        private bool CheckPermission(string roleName, ClaimsPrincipal user)
        {
            switch (roleName)
            {
                case Roles.Admin:
                    return user.HasPermissionClaim(Permissions.Users.AddAdministrator);
                case Roles.Anchor:
                    return user.HasPermissionClaim(Permissions.Anchors.Add);
                case Roles.Participant:
                    return user.HasPermissionClaim(Permissions.Participants.Add);
                default:
                    return false;
            }
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = request.GetUser();

            List<string> roleNames = await _context.Roles.Where(x => request.RolesId.Contains(x.Id)).Select(x => x.Name)
                .ToListAsync(cancellationToken);
            if (!String.IsNullOrEmpty(request.RoleName))
            {
                roleNames.Add(request.RoleName);
            }

            if (!CheckPermission(roleNames, request.User))
            {
                throw new UserAddException(PolishReadableMessage.User.NotPermissionToRole);
            }


            IdentityResult createUserResult = await _userManager.CreateAsync(user);

            if (!createUserResult.Succeeded)
            {
                throw new UserAddException(createUserResult.Errors.FirstOrDefault()?.Description ?? "Nie udało się dodać użytkownika");
            }

            foreach (var roleName in roleNames)
            {
                await _userManager.AddToRoleAsync(user, roleName);

                await _userManager.UpdateAsync(user);
            }


            if (!ShouldSendEmail(request, roleNames.Contains(Roles.Participant)))
            {
                return true;
            }

            string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string message = $@"Cześć,<p> 
                        witamy Cię w naszym systemie. Dla twojej poczty e-mail zostało przypisane nowe konto. Aby ustawić sobie hasło kliknij <a href='{request.UrlAddress}?emailConfirmationToken={emailConfirmationToken}&passwordToken={resetPasswordToken}&email={user.Email}'>tutaj</a>";


            await _emailSender.SendEmailAsync(user.Email, "Aktywacja konta w systemie", message);


            return true;
        }

        private static bool ShouldSendEmail(Command request, bool isParticipantRole)
        {
            return request.ShouldSendEmail && isParticipantRole;
        }
    }

    public class UserAddException : Exception
    {
        public UserAddException(string message):base(message)
        {
        }
    }
}
