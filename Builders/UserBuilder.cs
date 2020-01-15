using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Model;
using Model.Domain;

namespace Builders
{
    public class UserBuilder : Builder<UserBuilder, User>
    {
        private bool _userWasCreated = false;
        public UserBuilder(SchoolManagementContext context) : base(context)
        {
            State.Id = Guid.NewGuid().ToString();
        }

        public UserBuilder WithName(string firstName, string lastName)
        {
            State.FirstName = firstName;
            State.LastName = lastName;
            return this;
        }

        public UserBuilder WithUser(User user)
        {
            State = user;
            _userWasCreated = true;
            return this;
        }

        public UserBuilder WithRole(Action<RoleBuilder> builder)
        {
            RoleBuilder roleBuilder = new RoleBuilder(Context);
            builder.Invoke(roleBuilder);
            Role role = roleBuilder.Build();
            WithRole(role);
            if (!Context.Roles.Any(x => x.Name == role.Name))
            {
                Context.Add(role);
            }
            return this;
        }

        public UserBuilder WithRole(Role role)
        {
            IdentityUserRole<string> userRole = new IdentityUserRole<string>
            {
                UserId = State.Id,
                RoleId = role.Id
            };
            Context.Add(userRole);
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            State.Email = email;
            State.UserName = email;

            return this;
        }

        public override User BuildAndSave()
        {
            if (_userWasCreated)
            {
                return Build();
            }

            return base.BuildAndSave();
        }
    }
}
