using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomBannedUsersEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetRoomBannedUsersMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room instance = session.Habbo.CurrentRoom;
            if (instance == null || !instance.CheckRights(session, true))
                return;

            if (instance.GetBans().BannedUsers().Count > 0)
                session.SendPacket(new GetRoomBannedUsersComposer(instance));
        }
    }
}
