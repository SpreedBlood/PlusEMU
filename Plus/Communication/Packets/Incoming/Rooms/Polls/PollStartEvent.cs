using Plus.Communication.Packets.Outgoing.Rooms.Polls;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Polls
{
    class PollStartEvent : IPacketEvent
    {
        public int Header => -1; //TODO: Polls
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new PollContentsComposer());
        }
    }
}
