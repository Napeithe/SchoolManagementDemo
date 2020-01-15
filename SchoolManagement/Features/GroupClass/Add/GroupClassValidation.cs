using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace SchoolManagement.Features.GroupClass.Add
{
    public class GroupClassValidation : AbstractValidator<Command>
    {
        public GroupClassValidation()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MinimumLength(3);
            RuleFor(x => x.GroupLevelId).NotNull();
            RuleFor(x => x.ParticipantLimit).GreaterThan(0);
            RuleFor(x => x.RoomId).NotNull();
        }
    }
}
