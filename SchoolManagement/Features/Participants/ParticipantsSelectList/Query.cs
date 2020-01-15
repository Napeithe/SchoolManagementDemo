using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using SchoolManagement.Features.Shared.Components.SearchComponent;

namespace SchoolManagement.Features.Participants.ParticipantsSelectList
{
    public class Select2Items
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }


    public class Query : IRequest<Select2Results>
    {
        public string Term { get; set; } = "";
        public List<string> Exclude { get; set; } = new List<string>();
    }

    public class Handler : IRequestHandler<Query, Select2Results>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }
        public async Task<Select2Results> Handle(Query request, CancellationToken cancellationToken)
        {
            IQueryable<Select2Items> query = _context.UserRoles
                .Join(_context.Roles, userRole => userRole.RoleId, role => role.Id, (userRole, role) => new
                {
                    userRole.UserId,
                    role.Name
                }).Where(x => x.Name == Roles.Participant)
                .Join(_context.Users, user => user.UserId, user => user.Id, (_, user) => new Select2Items()
                {
                    Id = user.Id,
                    Text = $"{user.FirstName} {user.LastName}"
                });
            if (!string.IsNullOrEmpty(request.Term))
            {
                string term = request.Term.ToUpper();
                query = query.Where(x => x.Text.ToUpper().Contains(term));
            }

            if (request.Exclude.Any())
            {
                query = query.Where(x => !request.Exclude.Contains(x.Id));
            }

            List<Select2Items> result = await query.ToListAsync(cancellationToken);
            return new Select2Results()
            {
                Results = result
            };
        }
    }
}
