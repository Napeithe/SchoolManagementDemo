using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Model.Domain.Interface;

namespace Model.Domain
{
    public class User  : IdentityUser, IUtcOffset
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FacebookLink { get; set; }

        public List<Pass> Passes { get; set; }
        public int UtcOffsetInMinutes { get; set; }
    }
}
