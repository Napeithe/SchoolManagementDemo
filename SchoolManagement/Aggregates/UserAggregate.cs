using Model.Domain;
using SchoolManagement.Features.Users.Edit;

namespace SchoolManagement.Aggregates
{
    public class UserAggregate : Aggregate<UserAggregate, User>
    {
        public UserAggregate Update(IUpdateUser command)
        {
            State.FirstName = command.FirstName;
            State.LastName = command.LastName;
            State.Email = command.Email;
            State.EmailConfirmed = command.IsEmailActivated;
            return this;
        }
    }
}
