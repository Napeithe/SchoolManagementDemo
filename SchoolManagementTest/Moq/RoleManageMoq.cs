using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Model.Domain;
using Moq;

namespace SchoolManagementTest.Moq
{
    public class RoleManagerMoq : RoleManager<Role>
    {
        public RoleManagerMoq() : base(new Mock<IRoleStore<Role>>().Object, null, null, null, null)
        {
        }

        public RoleManagerMoq(IRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) :
            base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
