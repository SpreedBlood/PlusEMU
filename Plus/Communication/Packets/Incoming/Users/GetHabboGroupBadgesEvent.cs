using System.Collections.Generic;
using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    class GetHabboGroupBadgesEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetHabboGroupBadgesMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            Dictionary<int, string> badges = new Dictionary<int, string>();
            foreach (RoomUser user in room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (user.IsBot || user.IsPet || user.GetClient() == null || user.GetClient().Habbo == null)
                    continue;

                if (user.GetClient().Habbo.GetStats().FavouriteGroupId == 0 || badges.ContainsKey(user.GetClient().Habbo.GetStats().FavouriteGroupId))
                    continue;

                if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(user.GetClient().Habbo.GetStats().FavouriteGroupId, out Group group))
                    continue;

                if (!badges.ContainsKey(group.Id))
                    badges.Add(group.Id, group.Badge);
            }

            if (session.Habbo.GetStats().FavouriteGroupId > 0)
            {
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(session.Habbo.GetStats().FavouriteGroupId, out Group group))
                {
                    if (!badges.ContainsKey(group.Id))
                        badges.Add(group.Id, group.Badge);
                }
            }

            room.SendPacket(new HabboGroupBadgesComposer(badges));
            session.SendPacket(new HabboGroupBadgesComposer(badges));
        }
    }
}
