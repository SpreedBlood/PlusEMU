using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class UnbanUserFromRoomEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.UnbanUserFromRoomMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room instance = session.Habbo.CurrentRoom;
            if (instance == null || !instance.CheckRights(session, true))
                return;

            int userId = packet.PopInt();
            int roomId = packet.PopInt();

            if (instance.GetBans().IsBanned(userId))
            {
                instance.GetBans().Unban(userId);

                session.SendPacket(new UnbanUserFromRoomComposer(roomId, userId));
            }
        }
    }
}
