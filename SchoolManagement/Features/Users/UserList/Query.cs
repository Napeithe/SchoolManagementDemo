using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;

namespace SchoolManagement.Features.Users.UserList
{
    public class Query : IRequest<UsersListVierModel>
    {
        public bool IsIncludeAdministrators { get; set; }
    }

    public class Handler : IRequestHandler<Query, UsersListVierModel>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<UsersListVierModel> Handle(Query request, CancellationToken cancellationToken)
        {
            var usersRole = _context.UserRoles
                .Join(_context.Roles, ru=>ru.RoleId, r=>r.Id, (userRole, role)=>new
                {
                    RoleName = role.Name,
                    UserId = userRole.UserId
                }).Where(x => x.RoleName != Roles.SuperAdmin);

            if (!request.IsIncludeAdministrators)
            {
                usersRole = usersRole.Where(x => x.RoleName != Roles.Admin);
            }

            var users = (await usersRole
                .Join(_context.Users, roleUser => roleUser.UserId, user => user.Id, (roleUser, user)=>
                new UserDto()
            { 
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleName = roleUser.RoleName,
                Id = user.Id,
            }).GroupBy(x=>x.Id).ToListAsync(cancellationToken)).Select(x=>new UserDto()
            {
                Id = x.Key,
                LastName = x.First().LastName,
                FirstName = x.First().FirstName,
                Email = x.First().Email,
                IsEmailConfirmed = x.First().IsEmailConfirmed,
                RoleName = x.Select(r=>r.RoleName).Aggregate((r1,r2)=>$"{r1}, {r2}")
            }).ToList();


            foreach (UserDto userDto in users)
            {
                foreach (KeyValuePair<string, string> role in Roles.Descriptions)
                {
                    userDto.RoleName = userDto.RoleName.Replace(role.Key, role.Value);
                }
            }

            return new UsersListVierModel().WithUsers(users);
        }
    }
}
