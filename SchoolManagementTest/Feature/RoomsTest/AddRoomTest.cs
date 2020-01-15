using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using SchoolManagement.Features.Rooms.Add;
using Xunit;

namespace SchoolManagementTest.Feature.RoomsTest
{
    public class AddRoomTest
    {
        [Fact]
        public async Task ExecuteShouldAddRoom()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Command cmd = new Command
            {
                Name = "Sala wielka"
            };
            await new Handler(context).Handle(cmd, CancellationToken.None);

            Assert.NotNull(context.Rooms);
            Assert.NotEmpty(context.Rooms);
            Assert.Single(context.Rooms);
            Assert.Equal(cmd.Name, context.Rooms.First().Name);
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenAddDuplicateRoom()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            Room room = new RoomBuilder(context).WithName("Sala wielka").BuildAndSave();
            Command cmd = new Command
            {
                Name = room.Name
            };
            AddRoomException addRoomException = await Assert.ThrowsAsync<AddRoomException>(async()=> await new Handler(context).Handle(cmd, CancellationToken.None));
            Assert.Equal(PolishReadableMessage.Room.NameDuplicate, addRoomException.Message);
            Assert.Single(context.Rooms);
        }
    }
}
