using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Model.Domain;
using SchoolManagement.Features.Pass.UsePass;
using SchoolManagement.Infrastructure;

namespace SchoolManagement.Features.Presences.ChangePresence
{
    public class PresenceFacade
    {
        private readonly IMediator _mediator;

        public PresenceFacade(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DataResult> SetAsNotPresence(Command command, CancellationToken token)
        {

            switch (command.PresenceType)
            {
                case PresenceType.Member:
                    {
                        await _mediator.Send(command, token);

                        Pass.ReturnPass.Command returnPass = new Pass.ReturnPass.Command
                        {
                            ParticipantId = command.ParticipantId,
                            ClassTimeId = command.ClassTimeId
                        };

                        return await _mediator.Send(returnPass, token);
                    }
                case PresenceType.Help:
                    var helpCommand = new RemoveNewUserFromClassTime.Help.Command()
                    {
                        ParticipantId = command.ParticipantId,
                        ClassTimeId = command.ClassTimeId
                    };
                    return await _mediator.Send(helpCommand, token);
                default:
                    var makeUpCommand = new RemoveNewUserFromClassTime.Help.Command()
                    {
                        ParticipantId = command.ParticipantId,
                        ClassTimeId = command.ClassTimeId
                    };
                    await _mediator.Send(makeUpCommand, token);
                    Pass.ReturnPass.Command makeUpReturnPass = new Pass.ReturnPass.Command
                    {
                        ParticipantId = command.ParticipantId,
                        ClassTimeId = command.ClassTimeId
                    };
                    return await _mediator.Send(makeUpReturnPass, token);
            }
        }

        public async Task<DataResult<PassMessage>> SetAsPresence(Command command, CancellationToken token)
        {

            switch (command.PresenceType)
            {
                case PresenceType.Member:
                    {
                        DataResult<int> result = await _mediator.Send(command, token);
                        Pass.UsePass.Command usePass = new Pass.UsePass.Command
                        {
                            ParticipantClassTimeId = result.Data
                        };

                        DataResult<PassMessage> dataResult = await _mediator.Send(usePass, token);
                        return dataResult;
                    }
                case PresenceType.Help:

                    var helpCommand = new AddNewUserToClassTime.Help.Command()
                    {
                        ParticipantId = command.ParticipantId,
                        ClassTimeId = command.ClassTimeId
                    };
                    await _mediator.Send(helpCommand, token);
                    return DataResult<PassMessage>.Success(PassMessage.Success(""));
                default:

                    var makeUpCommand = new AddNewUserToClassTime.MakeUp.Command()
                    {
                        ParticipantId = command.ParticipantId,
                        ClassTimeId = command.ClassTimeId
                    };
                    DataResult<int> makeUpResult = await _mediator.Send(makeUpCommand, token);
                    Pass.UsePass.Command usePassForMakeUp = new Pass.UsePass.Command
                    {
                        ParticipantClassTimeId = makeUpResult.Data
                    };
                    return DataResult<PassMessage>.Success(PassMessage.Success(""));
            }
        }
    }
}
