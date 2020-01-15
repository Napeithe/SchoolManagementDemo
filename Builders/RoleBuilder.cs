using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;

namespace Builders
{
    public class RoleBuilder:Builder<RoleBuilder,Role>
    {
        public RoleBuilder(SchoolManagementContext context):base(context)
        {
            State.Id = Guid.NewGuid().ToString();
        }

        public RoleBuilder WithName(string name)
        {
            State.Name = name;
            return this;
        }

        public RoleBuilder WithDescription(string description)
        {
            State.Description = description;
            return this;
        }

        public RoleBuilder AddUserToRole(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new BuilderException("User has not set Id. Cannot add user to role");
            }

            IdentityUserRole<string> userRole = new IdentityUserRole<string>()
            {
                RoleId = State.Id,
                UserId = user.Id
            };

            Context.Add(userRole);

            return this;
        }
    }

    public class BuilderException : Exception
    {
        public BuilderException(string message):base(message)
        {
            
        }
    }
}
