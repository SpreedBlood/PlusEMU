using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CloseIssueDefaultActionEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.CloseIssueDefaultActionEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;
        }
    }
}
