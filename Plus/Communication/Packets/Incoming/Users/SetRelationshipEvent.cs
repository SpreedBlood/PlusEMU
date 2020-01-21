using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Relationships;

using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.Users.Messenger;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Users
{
    class SetRelationshipEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetRelationshipMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            int user = packet.PopInt();
            int type = packet.PopInt();

            if (!session.Habbo.GetMessenger().FriendshipExists(user))
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, you can only set a relationship where a friendship exists."));
                return;
            }

            if (type < 0 || type > 3)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Oops, you've chosen an invalid relationship type."));
                return;
            }

            if (session.Habbo.Relationships.Count > 2000)
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Sorry, you're limited to a total of 2000 relationships."));
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + session.Habbo.Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + session.Habbo.Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);
                    dbClient.RunQuery();

                    if (session.Habbo.Relationships.ContainsKey(user))
                        session.Habbo.Relationships.Remove(user);
                }
                else
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + session.Habbo.Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", user);
                    int id = dbClient.GetInteger();

                    if (id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + session.Habbo.Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", user);
                        dbClient.RunQuery();

                        if (session.Habbo.Relationships.ContainsKey(id))
                            session.Habbo.Relationships.Remove(id);
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + session.Habbo.Id + "', @target, @type)");
                    dbClient.AddParameter("target", user);
                    dbClient.AddParameter("type", type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());

                    if (!session.Habbo.Relationships.ContainsKey(user))
                        session.Habbo.Relationships.Add(user, new Relationship(newId, user, type));
                }

                GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(user);
                if (client != null)
                    session.Habbo.GetMessenger().UpdateFriend(user, client, true);
                else
                {
                    Habbo habbo = PlusEnvironment.GetHabboById(user);
                    if (habbo != null)
                    {
                        if (session.Habbo.GetMessenger().TryGetFriend(user, out MessengerBuddy buddy))
                            session.SendPacket(new FriendListUpdateComposer(session, buddy));
                    }
                }
            }
        }
    }
}
