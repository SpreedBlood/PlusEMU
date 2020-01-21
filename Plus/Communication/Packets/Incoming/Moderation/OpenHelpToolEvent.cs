using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class OpenHelpToolEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.OpenHelpToolMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new OpenHelpToolComposer());
        }
    }
}
