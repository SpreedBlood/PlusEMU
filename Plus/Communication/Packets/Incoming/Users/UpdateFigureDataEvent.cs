using System;
using System.Linq;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Users
{
    class UpdateFigureDataEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public UpdateFigureDataEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.UpdateFigureDataMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            string gender = packet.PopString().ToUpper();
            string look = PlusEnvironment.GetFigureManager().ProcessFigure(packet.PopString(), gender, session.Habbo.GetClothing().GetClothingParts, true);

            if (look == session.Habbo.Look)
                return;

            if ((DateTime.Now - session.Habbo.LastClothingUpdateTime).TotalSeconds <= 2.0)
            {
                session.Habbo.ClothingUpdateWarnings += 1;
                if (session.Habbo.ClothingUpdateWarnings >= 25)
                    session.Habbo.SessionClothingBlocked = true;
                return;
            }

            if (session.Habbo.SessionClothingBlocked)
                return;

            session.Habbo.LastClothingUpdateTime = DateTime.Now;

            string[] allowedGenders = { "M", "F" };
            if (!allowedGenders.Contains(gender))
            {
                session.SendPacket(new BroadcastMessageAlertComposer("Sorry, you chose an invalid gender."));
                return;
            }

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ProfileChangeLook);

            session.Habbo.Look = PlusEnvironment.FilterFigure(look);
            session.Habbo.Gender = gender.ToLower();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `look` = @look, `gender` = @gender WHERE `id` = '" + session.Habbo.Id + "' LIMIT 1");
                dbClient.AddParameter("look", look);
                dbClient.AddParameter("gender", gender);
                dbClient.RunQuery();
            }

            _achievementManager.ProgressAchievement(session, "ACH_AvatarLooks", 1);
            session.SendPacket(new AvatarAspectUpdateComposer(look, gender));
            if (session.Habbo.Look.Contains("ha-1006"))
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.WearHat);

            if (session.Habbo.InRoom)
            {
                RoomUser roomUser = session.Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
                if (roomUser != null)
                {
                    session.SendPacket(new UserChangeComposer(roomUser, true));
                    session.Habbo.CurrentRoom.SendPacket(new UserChangeComposer(roomUser, false));
                }
            }
        }
    }
}
