using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Model.DataSeeders
{
    public abstract class DataSeederCore
    {
        protected SchoolManagementContext Context { get; set; }

        protected DataSeederCore(SchoolManagementContext context)
        {
            Context = context;
        }

        public abstract bool DataExists();
        public abstract Task SeedData();
    }
}
