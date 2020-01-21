using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetMessengerInviteStatusEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetMessengerInviteStatusMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            bool status = packet.PopBoolean();

            session.Habbo.AllowMessengerInvites = status;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `ignore_invites` = @MessengerInvites WHERE `id` = '" + session.Habbo.Id + "' LIMIT 1");
                dbClient.AddParameter("MessengerInvites", PlusEnvironment.BoolToEnum(status));
                dbClient.RunQuery();
            }
        }
    }
}
