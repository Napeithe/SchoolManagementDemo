using System.Collections.Generic;
using System.Security.Claims;

namespace Model.Auth
{
    public static class RoleClaims
    {
        public static readonly List<Claim> SuperAdminClaims = new List<Claim>
        {
            new Claim(CustomClaimTypes.Permission, Permissions.Users.SeeAllUser),
            new Claim(CustomClaimTypes.Permission, Permissions.Users.AddAdministrator),
            new Claim(CustomClaimTypes.Permission, Permissions.Users.RemoveAdministrator),
            new Claim(CustomClaimTypes.Permission, Permissions.Users.ShowAdministrator),
            new Claim(CustomClaimTypes.Permission, Permissions.Users.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Users.RemoveFromSystem),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.Add),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.Add),

        };

        public static readonly List<Claim> AdminClaims = new List<Claim>
        {
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.AddRoom),
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.EditRoom),
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.RemoveRoom),
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.ShowRoomList),

            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.Detail),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.List),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.Add),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.Edit),
            new Claim(CustomClaimTypes.Permission, Permissions.Anchors.Remove),

            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.General),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.Add),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.Edit),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.Remove),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.List),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.Detail),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.GetParticipant),
            new Claim(CustomClaimTypes.Permission, Permissions.GroupClass.GetGroupMembers),


            new Claim(CustomClaimTypes.Permission, Permissions.Participants.GetToSelect),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.Add),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.Detail),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.List),
            new Claim(CustomClaimTypes.Permission, Permissions.Participants.Remove),

            new Claim(CustomClaimTypes.Permission, Permissions.Calendar.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Calendar.Show),
            new Claim(CustomClaimTypes.Permission, Permissions.Calendar.ChangeDate),

            new Claim(CustomClaimTypes.Permission, Permissions.Presence.ShowPresence),
            new Claim(CustomClaimTypes.Permission, Permissions.Presence.ChangePresence),

            new Claim(CustomClaimTypes.Permission, Permissions.Members.SetPassStatusAsPaid),
            new Claim(CustomClaimTypes.Permission, Permissions.Members.Update),
        };

        public static readonly List<Claim> AnchorClaims = new List<Claim>
        {
            new Claim(CustomClaimTypes.Permission, Permissions.Calendar.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Calendar.Show),

            new Claim(CustomClaimTypes.Permission, Permissions.Presence.ChangePresence),
            new Claim(CustomClaimTypes.Permission, Permissions.Presence.ShowPresence),
        };

        public static readonly List<Claim> ParticipantClaims = new List<Claim>
        {
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.General),
            new Claim(CustomClaimTypes.Permission, Permissions.Rooms.ShowRoomList)
        };
    }
}
