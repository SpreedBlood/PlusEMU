using Plus.Database;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Achievements.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementDao
    {
        public async Task<Dictionary<string, Achievement>> GetAchievementLevelsAsync()
        {
            Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
            using (IAsyncDbClient client = ConnectionProvider.GetConnection())
            {
                using (IQuery query = client.CreateQuery("SELECT `id`,`category`,`group_name`,`level`,`reward_pixels`,`reward_points`,`progress_needed`,`game_id` FROM `achievements`"))
                {
                    using (var reader = await query.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            uint id = reader.GetData<uint>("id");
                            string category = reader.GetData<string>("category");
                            string groupName = reader.GetData<string>("group_name");
                            int rewardPixels = reader.GetData<int>("reward_pixels");
                            int rewardPoints = reader.GetData<int>("reward_points");
                            int progressNeeded = reader.GetData<int>("progress_needed");

                            AchievementLevel level = new AchievementLevel(reader.GetData<int>("level"), rewardPixels, rewardPoints, progressNeeded);

                            if (!achievements.ContainsKey(groupName))
                            {
                                Achievement achievement = new Achievement((int)id, groupName, category, reader.GetData<int>("game_id"));
                                achievement.AddLevel(level);
                                achievements.Add(groupName, achievement);
                            }
                            else
                                achievements[groupName].AddLevel(level);
                        }
                    }
                }
            }

            return achievements;
        }

        internal async Task ProgressAchievement(int userId, string group, int newLevel, int newProgress)
        {
            using (IAsyncDbClient client = ConnectionProvider.GetConnection())
            {
                using (IQuery query = client.CreateQuery("REPLACE INTO `user_achievements` VALUES ('@0', @1, '@2', '@3')", userId, group, newLevel, newProgress))
                {
                    await query.ExecuteNonQueryAsync();
                }
            }
        }
    }
}