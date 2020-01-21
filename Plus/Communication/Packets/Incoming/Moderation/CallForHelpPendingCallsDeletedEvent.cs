using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class CallForHelpPendingCallsDeletedEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.CallForHelpPendingCallsDeletedMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            if (PlusEnvironment.GetGame().GetModerationManager().UserHasTickets(session.Habbo.Id))
            {
                ModerationTicket pendingTicket = PlusEnvironment.GetGame().GetModerationManager().GetTicketBySenderId(session.Habbo.Id);
                if (pendingTicket != null)
                {
                    pendingTicket.Answered = true;
                    PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.Habbo.Id, pendingTicket), "mod_tool");
                }
            }
        }
    }
}
