using System.Collections.Generic;

namespace SchoolManagement.Features.Users.UserList
{
    public class UsersListVierModel
    {
        public UsersListVierModel()
        {
            UserDtoList = new List<UserDto>();
        }

        public List<UserDto> UserDtoList { get; set; }

        public UsersListVierModel WithUsers(List<UserDto> users)
        {
            UserDtoList = users;
            return this;
        }
    }
}