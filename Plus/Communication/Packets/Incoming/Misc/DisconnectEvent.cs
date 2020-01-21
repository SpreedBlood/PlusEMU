using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class DisconnectEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.DisconnectionMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.Disconnect();
        }
    }
}
