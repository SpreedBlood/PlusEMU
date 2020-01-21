using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class MemoryPerformanceEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.MemoryPerformanceMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {

        }
    }
}
