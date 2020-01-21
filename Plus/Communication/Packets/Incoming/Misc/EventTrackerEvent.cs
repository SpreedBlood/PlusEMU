using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class EventTrackerEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.EventTrackerMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {

        }
    }
}
