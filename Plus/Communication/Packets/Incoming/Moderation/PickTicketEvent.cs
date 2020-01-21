using Plus.HabboHotel.Moderation;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class PickTicketEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.PickTicketMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.GetPermissions().HasRight("mod_tool"))
                return;

            packet.PopInt();//Junk
            int ticketId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(ticketId, out ModerationTicket ticket))
                return;

            ticket.Moderator = session.Habbo;
            PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.Habbo.Id, ticket), "mod_tool");
        }
    }
}
