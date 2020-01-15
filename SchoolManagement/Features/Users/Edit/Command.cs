using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Aggregates;
using SchoolManagement.Features.Users.UserList;

namespace SchoolManagement.Features.Users.Edit
{

    public class Command : IRequest<IdentityResult>, IUpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> RolesId { get; set; } = new List<string>();
        public bool IsEmailActivated { get; set; }
        public string Id { get; set; }

        public static Command FromUserDto(UserDto userDto)
        {
            return new Command()
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                RolesId = userDto.RolesIds,
                Email = userDto.Email,
                IsEmailActivated = userDto.IsEmailConfirmed
            };
        }

    }

    public interface IUpdateUser
    {
       string FirstName { get; set; }
       string LastName { get; set; }
       string Email { get; set; }
       List<string> RolesId { get; set; }
       bool IsEmailActivated { get; set; }
       string Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, IdentityResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly SchoolManagementContext _context;

        public Handler(UserManager<User> userManager, SchoolManagementContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IdentityResult> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            var allUserRoles = await _context.UserRoles.Where(x=>x.UserId == request.Id)
                .Join(_context.Roles, ur=>ur.RoleId, r=>r.Id, (ur, r)=> new {r.Id, r.Name})
                .ToListAsync(cancellationToken);

            List<string> rolesToRemove = allUserRoles.Where(x=>!request.RolesId.Contains(x.Id)).Select(x=>x.Name).ToList();

            List<string> rolesId = allUserRoles.Select(x => x.Id).ToList();

            List<string> rolesToAddIds = request.RolesId.Where(x => !rolesId.Contains(x)).ToList();

            List<string> rolesToAddNames = await _context.Roles.Where(x => rolesToAddIds.Contains(x.Id))
                .Select(x => x.Name).ToListAsync(cancellationToken);

            foreach (string roleName in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }

            foreach (string roleName in rolesToAddNames)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
            user = UserAggregate.FromState(user).Update(request).State;

            IdentityResult updateResult = await _userManager.UpdateAsync(user);

            return updateResult;
        }
    }
}
