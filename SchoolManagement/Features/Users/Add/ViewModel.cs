using System.Security.Claims;
using Model.Domain;

namespace SchoolManagement.Features.Users.Add
{
    public class ViewModel
    {
        private readonly ClaimsPrincipal _userClaims;

        public ViewModel(ClaimsPrincipal userClaims)
        {
            _userClaims = userClaims;
            CanChooseRole = userClaims.IsInRole(Roles.SuperAdmin);
        }

        public string Error { get; set; }
        public bool CanChooseRole { get; set; }
        public bool RoleName { get; set; }
        public Command User { get; set; } = new Command();
    }
}
