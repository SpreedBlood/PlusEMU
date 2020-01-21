using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationCautionEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ModerationCautionMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.GetPermissions().HasRight("mod_caution"))
                return;

            int userId = packet.PopInt();
            string message = packet.PopString();

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.Habbo == null)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `cautions` = `cautions` + '1' WHERE `user_id` = '" + client.Habbo.Id + "' LIMIT 1");
            }

            client.SendNotification(message);
        }
    }
}
