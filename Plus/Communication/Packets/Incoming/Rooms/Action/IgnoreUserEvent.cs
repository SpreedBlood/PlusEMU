using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Outgoing.Rooms.Action;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class IgnoreUserEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public IgnoreUserEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.IgnoreUserMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            string username = packet.PopString();

            Habbo player = PlusEnvironment.GetHabboByUsername(username);
            if (player == null || player.GetPermissions().HasRight("mod_tool"))
                return;

            if (session.Habbo.GetIgnores().TryGet(player.Id))
                return;

            if (session.Habbo.GetIgnores().TryAdd(player.Id))
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_ignores` (`user_id`,`ignore_id`) VALUES(@uid,@ignoreId);");
                    dbClient.AddParameter("uid", session.Habbo.Id);
                    dbClient.AddParameter("ignoreId", player.Id);
                    dbClient.RunQuery();
                }

                session.SendPacket(new IgnoreStatusComposer(1, player.Username));

                _achievementManager.ProgressAchievement(session, "ACH_SelfModIgnoreSeen", 1);
            }
        }
    }
}
