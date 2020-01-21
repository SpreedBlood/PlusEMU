namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class InitializeGameCenterEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.InitializeGameCenterMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

        }
    }
}
