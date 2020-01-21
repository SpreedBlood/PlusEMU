using System;
using System.Linq;
using System.Collections.Generic;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using log4net;
using Plus.HabboHotel.Achievements.Models;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Achievements.AchievementManager");

        private readonly AchievementDao _dao;

        public Dictionary<string, Achievement> Achievements { get; private set; }

        public AchievementManager(AchievementDao achievementDao)
        {
            _dao = achievementDao;
        }

        public async void Init()
        {
            Achievements = await _dao.GetAchievementLevelsAsync();
        }

        public async void ProgressAchievement(GameClient session, string group, int progress, bool fromBeginning = false)
        {
            if (!Achievements.ContainsKey(group) || session == null)
                return;

            Achievement data = Achievements[group];
            if (data == null)
            {
                return;
            }

            UserAchievement userData = session.GetHabbo().GetAchievementData(group);
            if (userData == null)
            {
                userData = new UserAchievement(group, 0, 0);
                session.GetHabbo().Achievements.TryAdd(group, userData);
            }

            int totalLevels = data.Levels.Count;

            if (userData.Level == totalLevels)
                return; // done, no more.

            int targetLevel = userData.Level + 1;

            if (targetLevel > totalLevels)
                targetLevel = totalLevels;

            AchievementLevel level = data.Levels[targetLevel];
            int newProgress;
            if (fromBeginning)
                newProgress = progress;
            else
                newProgress = userData.Progress + progress;

            int newLevel = userData.Level;
            int newTarget = newLevel + 1;

            if (newTarget > totalLevels)
                newTarget = totalLevels;

            if (newProgress >= level.Requirement)
            {
                newLevel++;
                newTarget++;
                newProgress = 0;

                if (targetLevel == 1)
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + targetLevel, true, session);
                else
                {
                    session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(group + (targetLevel - 1)));
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + targetLevel, true, session);
                }

                if (newTarget > totalLevels)
                {
                    newTarget = totalLevels;
                }

                session.SendPacket(new AchievementUnlockedComposer(data, targetLevel, level.RewardPoints, level.RewardPixels));
                session.GetHabbo().GetMessenger().BroadcastAchievement(session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.AchievementUnlocked, group + targetLevel);

                await _dao.ProgressAchievement(session.GetHabbo().Id, group, newLevel, newProgress);

                userData.Level = newLevel;
                userData.Progress = newProgress;

                session.GetHabbo().Duckets += level.RewardPixels;
                session.GetHabbo().GetStats().AchievementPoints += level.RewardPoints;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, level.RewardPixels));
                session.SendPacket(new AchievementScoreComposer(session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel newLevelData = data.Levels[newTarget];
                session.SendPacket(new AchievementProgressedComposer(data, newTarget, newLevelData, totalLevels, session.GetHabbo().GetAchievementData(group)));

                return;
            }
            else
            {
                userData.Level = newLevel;
                userData.Progress = newProgress;

                await _dao.ProgressAchievement(session.GetHabbo().Id, group, newLevel, newProgress);

                session.SendPacket(new AchievementProgressedComposer(data, targetLevel, level, totalLevels, session.GetHabbo().GetAchievementData(group)));
            }
            return;
        }

        public ICollection<Achievement> GetGameAchievements(int gameId)
        {
            List<Achievement> achievements = new List<Achievement>();

            foreach (Achievement achievement in Achievements.Values.ToList())
            {
                if (achievement.Category == "games" && achievement.GameId == gameId)
                    achievements.Add(achievement);
            }

            return achievements;
        }
    }
}