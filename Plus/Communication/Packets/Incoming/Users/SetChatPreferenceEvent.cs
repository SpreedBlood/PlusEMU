using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetChatPreferenceEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetChatPreferenceMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            bool preference = packet.PopBoolean();

            session.Habbo.ChatPreference = preference;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `chat_preference` = @chatPreference WHERE `id` = '" + session.Habbo.Id + "' LIMIT 1");
                dbClient.AddParameter("chatPreference", PlusEnvironment.BoolToEnum(preference));
                dbClient.RunQuery();
            }
        }
    }
}
