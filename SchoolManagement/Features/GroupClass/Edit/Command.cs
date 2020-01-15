using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Domain;
using Model.Dto;
using SchoolManagement.Aggregates;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.GroupClass.Edit
{
    public class Command : IRequest<DataResult>, IUpdateGroupClass
    {
        public int GroupClassId { get; set; }
        public string Name { get; set; }
        public int? GroupLevelId { get; set; }
        public int? RoomId { get; set; }
        public int ParticipantLimit { get; set; }
        public bool IsSolo { get; set; }
        public List<string> Anchors { get; set; } = new List<string>();
        public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
        public DateTime Start { get; set; }
        public List<ClassDayOfWeekDto> DayOfWeeks { get; set; } = new List<ClassDayOfWeekDto>();
        public int DurationTimeInMinutes { get; set; }
        public int NumberOfClasses { get; set; }
        public int PassPrice { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }

    public class Handler : IRequestHandler<Command, DataResult>
    {
        private readonly SchoolManagementContext _context;

        public Handler(SchoolManagementContext context)
        {
            _context = context;
        }

        public async Task<DataResult> Handle(Command request, CancellationToken cancellationToken)
        {
            Model.Domain.GroupClass groupClass = await _context.GroupClass
                .Include(x => x.Anchors)
                .ThenInclude(x => x.User)
                .Include(x => x.GroupLevel)
                .Include(x => x.Participants)
                .ThenInclude(x=>x.User)
                .Include(x => x.Room)
                .Include(x=>x.Schedule)
                .Include(x=>x.ClassDaysOfWeek)
                .Where(x => x.Id == request.GroupClassId)
                .FirstOrDefaultAsync(cancellationToken);

            GroupClassAggregate groupClassAggregate = GroupClassAggregate.FromState(groupClass).UpdateAll(request);
       

            await UpdateAnchors(request, groupClassAggregate, cancellationToken);

            await UpdateRoom(request, groupClass, groupClassAggregate, cancellationToken);

            await UpdateGroupLevel(request, groupClass, groupClassAggregate, cancellationToken);

            await UpdateMembers(request, groupClassAggregate, cancellationToken);

            groupClassAggregate.UpdateDaysOfWeek(request, _context).CreateSchedule(request);

            _context.GroupClass.Update(groupClass);
            await _context.SaveChangesAsync(cancellationToken);
            return DataResult.Success();
        }

        private async Task UpdateGroupLevel(Command request, Model.Domain.GroupClass groupClass, GroupClassAggregate groupClassAggregate,
            CancellationToken cancellationToken)
        {
            if (request.GroupLevelId != default && request.GroupLevelId != groupClass.GroupLevel?.Id)
            {
                GroupLevel groupLevel = await _context.GroupLevel.Where(x => x.Id == request.GroupLevelId.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                groupClassAggregate.WithGroupLevel(groupLevel);
            }
        }

        private async Task UpdateRoom(Command request, Model.Domain.GroupClass groupClass, GroupClassAggregate groupClassAggregate,
            CancellationToken cancellationToken)
        {
            if (request.RoomId != default && request.RoomId != groupClass.Room?.Id)
            {
                Room room = await _context.Rooms.Where(x => x.Id == request.RoomId.Value)
                    .FirstOrDefaultAsync(cancellationToken);
                groupClassAggregate.WithRoom(room);
            }
        }

        private async Task UpdateMembers(Command request, GroupClassAggregate groupClassAggregate, CancellationToken cancellationToken)
        {
            List<string> currentIds = groupClassAggregate.GetCurrentParticipants().Select(x => x.UserId).ToList();
            var updateMembers = request.Participants.Select(x=>x.Id).ToList();

            RemoveMembers(groupClassAggregate, currentIds, updateMembers);
            await AddNewMembers(request, groupClassAggregate, currentIds, updateMembers, cancellationToken);
            groupClassAggregate.ChangeParticipantRole(request);
        }

        private void RemoveMembers(GroupClassAggregate groupClassAggregate, List<string> currentIds, List<string> updateMembers)
        {
            List<string> removedIds = GetRemovedIds(currentIds, updateMembers);
            removedIds.ForEach(x => groupClassAggregate.RemoveParticipant(x));
        }

        private async Task AddNewMembers(Command request, GroupClassAggregate groupClassAggregate, List<string> currentIds, List<string> updateMembers,
            CancellationToken cancellationToken)
        {
            List<string> newParticipantIds = GetNewIds(currentIds, updateMembers);
            List<User> newMembers =
                await _context.Users.Where(x => newParticipantIds.Contains(x.Id)).ToListAsync(cancellationToken);

            newMembers.ForEach(member =>
            {
                PassAggregate passAggregate = PassAggregate.New()
                    .UpdateByCommand(request)
                    .WithParticipant(member);
                ParticipantDto participantDto = request.Participants.First(p => p.Id == member.Id);
                groupClassAggregate.AddParticipant(member, participantDto.Role, passAggregate);
            });
        }


        private async Task UpdateAnchors(Command request, GroupClassAggregate groupClassAggregate, CancellationToken cancellationToken)
        {
            List<string> currentIds = groupClassAggregate.GetCurrentAnchors().Select(x => x.UserId).ToList();

            RemoveAnchors(request, groupClassAggregate, currentIds);

            await AddNewAnchors(request, groupClassAggregate, currentIds, cancellationToken);
        }

        private async Task AddNewAnchors(Command request, GroupClassAggregate groupClassAggregate, List<string> currentIds,
            CancellationToken cancellationToken)
        {
            List<string> newAnchorsIds = GetNewIds(currentIds, request.Anchors);
            List<User> newAnchors =
                await _context.Users.Where(x => newAnchorsIds.Contains(x.Id)).ToListAsync(cancellationToken);
            groupClassAggregate.AddAnchor(newAnchors);
        }

        private void RemoveAnchors(Command request, GroupClassAggregate groupClassAggregate, List<string> currentIds)
        {
            List<string> removedIds = GetRemovedIds(currentIds, request.Anchors);
            removedIds.ForEach(x => groupClassAggregate.RemoveAnchor(x));
        }

        private List<string> GetRemovedIds(List<string> currentIds, List<string> updatedIds)
        {
            return currentIds.Where(x => updatedIds.All(z => z != x)).ToList();
        }

        private List<string> GetNewIds(List<string> currentIds, List<string> updatedIds)
        {
            return updatedIds.Where(x => currentIds.All(z => z != x)).ToList();
        }
    }
}
