using System.Collections.Generic;

namespace SchoolManagement.Features.Users.UserList
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public List<string> RolesIds { get; set; }
        public bool IsEmailConfirmed { get; set; }
    }
}