using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using SchoolManagement.Features.Rooms.Remove;
using Xunit;

namespace SchoolManagementTest.Feature.RoomsTest
{
    public class RemoveTest
    {
        [Fact]
        public async Task ShouldRemoveRoom()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Room room = new RoomBuilder(context).WithName("SalaWielka").BuildAndSave();
            Command cmd = new Command
            {
                Id = room.Id
            };

            await new Handler(context).Handle(cmd, CancellationToken.None);

            Assert.Empty(context.Rooms);
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenRoomIsNotFound()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Command cmd = new Command
            {
                Id = 2
            };

            RemoveRoomException removeRoomException = await Assert.ThrowsAsync<RemoveRoomException>(async()=>await new Handler(context).Handle(cmd, CancellationToken.None));

            Assert.Equal(PolishReadableMessage.Room.RoomNotExist, removeRoomException.Message);
        }
    }
}
