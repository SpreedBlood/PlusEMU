using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Groups;

using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class JoinGroupEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.JoinGroupMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(packet.PopInt(), out Group group))
                return;

            if (group.IsMember(session.Habbo.Id) || group.IsAdmin(session.Habbo.Id) || (group.HasRequest(session.Habbo.Id) && group.Type == GroupType.Private))
                return;

            List<Group> groups = PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.Habbo.Id);
            if (groups.Count >= 1500)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, it appears that you've hit the group membership limit! You can only join upto 1,500 groups."));
                return;
            }

            group.AddMember(session.Habbo.Id);

            if (group.Type == GroupType.Locked)
            {
                List<GameClient> groupAdmins = (from client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList() where client != null && client.Habbo != null && @group.IsAdmin(client.Habbo.Id) select client).ToList();
                foreach (GameClient client in groupAdmins)
                {
                    client.SendPacket(new GroupMembershipRequestedComposer(group.Id, session.Habbo, 3));
                }

                session.SendPacket(new GroupInfoComposer(group, session));
            }
            else
            {
                session.SendPacket(new GroupFurniConfigComposer(PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.Habbo.Id)));
                session.SendPacket(new GroupInfoComposer(group, session));

                if (session.Habbo.CurrentRoom != null)
                    session.Habbo.CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
                else
                    session.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
            }
        }
    }
}
