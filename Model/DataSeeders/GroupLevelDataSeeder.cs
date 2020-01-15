using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model.Domain;

namespace Model.DataSeeders
{
    public class GroupLevelDataSeeder : DataSeederCore
    {

        public GroupLevelDataSeeder(SchoolManagementContext context) : base(context)
        {
        }

        public override bool DataExists()
        {
            return Context.GroupLevel.Any();
        }

        public override async Task SeedData()
        {
            List<GroupLevel> levels = new List<GroupLevel>
            {
                new GroupLevel()
                {
                    Name = "Początkujący",
                    Level = 0
                },
                new GroupLevel()
                {
                    Name = "Średnio zaawansowany",
                    Level = 1
                },
                new GroupLevel()
                {
                    Name = "Zaawansowany",
                    Level = 2
                }
            };
            await Context.AddRangeAsync(levels);
            await Context.SaveChangesAsync();
        }
    }
}
