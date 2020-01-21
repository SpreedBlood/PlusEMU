using System;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    class GroupInfoComposer : ServerPacket
    {
        public GroupInfoComposer(Group Group, GameClient Session, bool NewWindow = false)
            : base(ServerPacketHeader.GroupInfoMessageComposer)
        {
            DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Group.CreateTime);

            WriteInteger(Group.Id);
            WriteBoolean(true);
            WriteInteger(Group.Type == GroupType.Open ? 0 : Group.Type == GroupType.Locked ? 1 : 2);
            WriteString(Group.Name);
            WriteString(Group.Description);
            WriteString(Group.Badge);
            WriteInteger(Group.RoomId);
            WriteString(Group.GetRoom() != null ? Group.GetRoom().Name : "No room found..");    // room name
            WriteInteger(Group.CreatorId == Session.Habbo.Id ? 3 : Group.HasRequest(Session.Habbo.Id) ? 2 : Group.IsMember(Session.Habbo.Id) ? 1 : 0);
            WriteInteger(Group.MemberCount); // Members
            WriteBoolean(false);//?? CHANGED
            WriteString(Origin.Day + "-" + Origin.Month + "-" + Origin.Year);
            WriteBoolean(Group.CreatorId == Session.Habbo.Id);
            WriteBoolean(Group.IsAdmin(Session.Habbo.Id)); // admin
            WriteString(PlusEnvironment.GetUsernameById(Group.CreatorId));
            WriteBoolean(NewWindow); // Show group info
            WriteBoolean(Group.AdminOnlyDeco == 0); // Any user can place furni in home room
            WriteInteger(Group.CreatorId == Session.Habbo.Id ? Group.RequestCount : Group.IsAdmin(Session.Habbo.Id) ? Group.RequestCount : Group.IsMember(Session.Habbo.Id) ? 0 : 0); // Pending users
            //base.WriteInteger(0);//what the fuck
            WriteBoolean(Group != null ? Group.ForumEnabled : true);//HabboTalk.
        }
    }
}