using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Model.Domain;

namespace SchoolManagement.Features.RoleAssign.Assign
{
    public class RoleAssignViewModel
    {
        public string ReturnUrl { get; set; }
        public List<SelectListItem> Users { get; set; }
        private string _roleName;
        public string RoleName
        {
            get => _roleName;
            set
            {
                _roleName = value;
                if (value != null)
                {
                    RoleDescription = Roles.Descriptions[value];
                }
            }
        }

        public string RoleDescription { get; private set; }
    }
}
