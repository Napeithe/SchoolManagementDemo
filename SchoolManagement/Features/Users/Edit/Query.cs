using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using SchoolManagement.Features.Users.UserList;

namespace SchoolManagement.Features.Users.Edit
{
    public class Query : IRequest<UserDto>
    {
        public string Id { get; set; }
    }

    public class QueryHandler : IRequestHandler<Query, UserDto>
    {
        private readonly SchoolManagementContext _context;

        public QueryHandler(SchoolManagementContext context)
        {
            _context = context;
        }
        public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
        {
            UserDto userDto = await _context.Users.Where(x => x.Id == request.Id).Select(x => new UserDto()
            {
                Email = x.Email,
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IsEmailConfirmed = x.EmailConfirmed 
            }).FirstOrDefaultAsync(cancellationToken);

            if (userDto is null)
            {
                throw new EditUserException(PolishReadableMessage.User.CannotFindUser);
            }

            List<string> rolesIds = await _context.UserRoles.Where(x=>x.UserId == request.Id).Select(x=>x.RoleId).ToListAsync(cancellationToken);
            userDto.RolesIds = rolesIds;

            return userDto;
        }
    }

    public class EditUserException : Exception
    {
        public EditUserException(string message) : base(message)
        {
            
        }
    }

}
