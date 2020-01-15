using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Model.Domain
{
    public class Role : IdentityRole
    {
        public string Description { get; set; }
        public Role()
        {
            
        }

        public Role(string name, string description) : base(name)
        {
            Description = description;
        }
    }



    public static class Roles
    {
        public const string SuperAdmin = "super_admin";
        public const string Admin = "admin";
        public const string Anchor = "anchor";
        public const string Participant = "participant";

        public static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>()
        {
            {SuperAdmin, "Super administrator"},
            {Admin, "Administrator"},
            {Anchor, "Prowadzący"},
            {Participant, "Uczestnik"},
        };
    }
}
