using System;
using Plus.Utilities;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Database.Interfaces;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Rooms.Avatar
{
    class ChangeMottoEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public ChangeMottoEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.ChangeMottoMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.Habbo.TimeMuted > 0)
            {
                session.SendNotification("Oops, you're currently muted - you cannot change your motto.");
                return;
            }

            if ((DateTime.Now - session.Habbo.LastMottoUpdateTime).TotalSeconds <= 2.0)
            {
                session.Habbo.MottoUpdateWarnings += 1;
                if (session.Habbo.MottoUpdateWarnings >= 25)
                    session.Habbo.SessionMottoBlocked = true;
                return;
            }

            if (session.Habbo.SessionMottoBlocked)
                return;

            session.Habbo.LastMottoUpdateTime = DateTime.Now;

            string newMotto = StringCharFilter.Escape(packet.PopString().Trim());

            if (newMotto.Length > 38)
                newMotto = newMotto.Substring(0, 38);

            if (newMotto == session.Habbo.Motto)
                return;

            if (!session.Habbo.GetPermissions().HasRight("word_filter_override"))
                newMotto = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(newMotto);

            session.Habbo.Motto = newMotto;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `motto` = @motto WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", session.Habbo.Id);
                dbClient.AddParameter("motto", newMotto);
                dbClient.RunQuery();
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ProfileChangeMotto);
            _achievementManager.ProgressAchievement(session, "ACH_Motto", 1);

            if (session.Habbo.InRoom)
            {
                Room room = session.Habbo.CurrentRoom;
                if (room == null)
                    return;

                RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
                if (user == null || user.GetClient() == null)
                    return;

                room.SendPacket(new UserChangeComposer(user, false));
            }
        }
    }
}
