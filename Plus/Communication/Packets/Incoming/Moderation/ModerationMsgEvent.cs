using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationMsgEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ModerationMsgMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.GetPermissions().HasRight("mod_alert"))
                return;

            int userId = packet.PopInt();
            string message = packet.PopString();

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null)
                return;

            client.SendNotification(message);
        }
    }
}
