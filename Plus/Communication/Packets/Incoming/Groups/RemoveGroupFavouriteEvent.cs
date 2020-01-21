using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Groups
{
    class RemoveGroupFavouriteEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.RemoveGroupFavouriteMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.Habbo.GetStats().FavouriteGroupId = 0;

            if (session.Habbo.InRoom)
            {
                RoomUser user = session.Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
                if (user != null)
                    session.Habbo.CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(null, user.VirtualId));
                session.Habbo.CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
            }
            else
                session.SendPacket(new RefreshFavouriteGroupComposer(session.Habbo.Id));
        }
    }
}
