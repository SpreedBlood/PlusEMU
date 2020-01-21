using System.Linq;
using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;


namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RemoveBuddyEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.RemoveBuddyMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            int amount = packet.PopInt();
            if (amount > 100)
                amount = 100;
            else if (amount < 0)
                return;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                for (int i = 0; i < amount; i++)
                {
                    int id = packet.PopInt();

                    if (session.Habbo.Relationships.Count(x => x.Value.UserId == id) > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = @id AND `target` = @target OR `target` = @id AND `user_id` = @target");
                        dbClient.AddParameter("id", session.Habbo.Id);
                        dbClient.AddParameter("target", id);
                        dbClient.RunQuery();
                    }

                    if (session.Habbo.Relationships.ContainsKey(id))
                        session.Habbo.Relationships.Remove(id);

                    GameClient target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(id);
                    if (target != null)
                    {
                        if (target.Habbo.Relationships.ContainsKey(session.Habbo.Id))
                            target.Habbo.Relationships.Remove(session.Habbo.Id);
                    }

                    session.Habbo.GetMessenger().DestroyFriendship(id);
                }
            }
        }
    }
}
