using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model.Domain;

namespace SchoolManagement.Features.Participants.Role
{
    public class Query : IRequest<List<SelectListItem>>
    {
    }

    public class Handler : IRequestHandler<Query, List<SelectListItem>>
    {
        public Task<List<SelectListItem>> Handle(Query request, CancellationToken cancellationToken)
        {
            List<SelectListItem> selectListItems = Enum.GetValues(typeof(ParticipantRole)).Cast<ParticipantRole>()
                .Select(x => new SelectListItem(x.ToString(), ((int) x).ToString())).ToList();
            return Task.FromResult(selectListItems);
        }
    }
}
