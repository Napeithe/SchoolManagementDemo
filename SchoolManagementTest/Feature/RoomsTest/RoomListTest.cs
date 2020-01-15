using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Domain;
using SchoolManagement.Features.Rooms.List;
using Xunit;

namespace SchoolManagementTest.Feature.RoomsTest
{
    public class RoomListTest
    {
        [Fact]
        public async Task ExecuteShouldReturnRoomList()
        {
            //Arrange
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new RoomBuilder(context).WithName("Sala wielka").BuildAndSave();
            Room expected = new RoomBuilder(context).WithName("Sala biała").BuildAndSave();
            Query query = new Query();
            //Act
            List<RoomDto> result = await new Handler(context).Handle(query, CancellationToken.None);
            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            RoomDto resultItem = result.First();
            Assert.Equal(expected.Name, resultItem.Name);
            Assert.Equal(expected.Id, resultItem.Id);
        }
    }
}
