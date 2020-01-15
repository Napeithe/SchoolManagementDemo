namespace Model.Auth
{
    public class Permissions
    {
        public static class Users
        {
            public const string General = "users.general";
            public const string AddAdministrator = "users.add_administrator";
            public const string RemoveAdministrator = "users.remove_administrator";
            public const string ShowAdministrator = "users.show_administrator";
            public const string AddAnchor = "users.add_anchor";
            public const string RemoveAnchor= "users.remove_anchor";
            public const string SeeAllUser = "users.seeAllUser";
            public const string RemoveFromSystem = "user.removefromsystem";
        }

        public static class Rooms
        {
            public const string General = "rooms.general";
            public const string ShowRoomList = "rooms.show_room_list";
            public const string AddRoom = "rooms.add_room";
            public const string RemoveRoom = "rooms.remove_room";
            public const string EditRoom = "rooms.edit_room";

        }

        public static class Anchors
        {
            public const string General = "anchors.general";
            public const string List = "anchors.list";
            public const string Detail = "anchors.detail";
            public const string Add = "anchors.add";
            public const string Edit = "anchors.edit";
            public const string Remove = "anchors.remove";
        }

        public static class GroupClass
        {
            public const string General = "groupclass.general";
            public const string List = "groupclass.list";
            public const string Detail = "groupclass.detail";
            public const string Add = "groupclass.add";
            public const string Edit = "groupclass.edit";
            public const string Remove = "groupclass.remove";
            public const string GetParticipant = "groupclass.getparticipant";
            public const string GetGroupMembers = "groupclass.getgroupmembers";
        }

        public static class Participants
        {
            public const string Add = "participants.add";
            public const string GetToSelect = "participants.gettoselect";
            public const string General = "participants.general";
            public const string List = "participants.list";
            public const string Detail = "participants.detail";
            public const string Remove = "participants.remove";
        }

        public static class Members
        {
            public const string SetPassStatusAsPaid = "members.setpassstatusaspaid";
            public const string Update = "members.update";
        }

        public static class Calendar
        {
            public const string Show = "calendar.show"; 
            public const string General = "calendar.general";
            public const string ChangeDate = "calendar.changedate";
        }

        public static class Presence
        {
            public const string ChangePresence = "presence.changepresence";
            public const string ShowPresence = "presence.showpresence";
        }
    }
}
