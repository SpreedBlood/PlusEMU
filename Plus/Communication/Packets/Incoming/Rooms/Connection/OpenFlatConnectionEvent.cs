using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Connection
{
    public class OpenFlatConnectionEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.OpenFlatConnectionMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            int roomId = packet.PopInt();
            string password = packet.PopString();

            session.Habbo.PrepareRoom(roomId, password);
        }
    }
}
