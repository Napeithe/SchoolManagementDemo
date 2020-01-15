using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Builders;
using Model;
using Model.Dto;
using Model.Dto.GroupClasses;
using SchoolManagement.Features.GroupClass.List;
using Xunit;

namespace SchoolManagementTest.Feature.GroupClassTest
{
    public class ListTest
    {
        [Fact]
        public async Task ShouldReturnGroupClassName()
        {
            SchoolManagementContext context = new ContextBuilder().BuildClean();
            new GroupClassBuilder(context).BuildAndSave();
            new GroupClassBuilder(context).BuildAndSave();
            new GroupClassBuilder(context).BuildAndSave();
            var query = new Query();
            //Act
            List<GroupClassItemDto> result = await new Handler(context).Handle(query, CancellationToken.None);
            //Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        }
    }
}
