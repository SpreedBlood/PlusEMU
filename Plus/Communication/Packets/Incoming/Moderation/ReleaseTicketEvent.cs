using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ReleaseTicketEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ReleaseTicketMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.GetPermissions().HasRight("mod_tool"))
                return;

            int amount = packet.PopInt();

            for (int i = 0; i < amount; i++)
            {
                if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(packet.PopInt(), out ModerationTicket ticket))
                    continue;

                ticket.Moderator = null;
                PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.Habbo.Id, ticket), "mod_tool");
            }
        }
    }
}
