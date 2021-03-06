using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.UpdateNavigatorSettingsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();
            if (roomId == 0)
                return;

            if (!RoomFactory.TryGetData(roomId, out RoomData _))
                return;

            session.Habbo.HomeRoom = roomId;
            session.SendPacket(new NavigatorSettingsComposer(roomId));
        }
    }
}
