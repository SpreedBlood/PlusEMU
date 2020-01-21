using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class SetGroupFavouriteEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetGroupFavouriteMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            int groupId = packet.PopInt();
            if (groupId == 0)
                return;

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            session.Habbo.GetStats().FavouriteGroupId = group.Id;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_stats` SET `groupid` = @groupId WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("groupId", session.Habbo.GetStats().FavouriteGroupId);
                dbClient.AddParameter("userId", session.Habbo.Id);
                dbClient.RunQuery();
            }

            if (session.Habbo.InRoom && session.Habbo.CurrentRoom != null)
            {
                session.Habbo.CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
                session.Habbo.CurrentRoom.SendPacket(new HabboGroupBadgesComposer(group));

                RoomUser user = session.Habbo.CurrentRoom.GetRoomUserManager()
                    .GetRoomUserByHabbo(session.Habbo.Id);
                if (user != null)
                    session.Habbo.CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
            }
            else
                session.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
        }
    }
}
