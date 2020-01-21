using Plus.Database;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Badges.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Badges
{
    internal class BadgeDao
    {
        internal async Task<Dictionary<string, BadgeDefinition>> GetBadges()
        {
            Dictionary<string, BadgeDefinition> badges = new Dictionary<string, BadgeDefinition>();
            using (IAsyncDbClient client = ConnectionProvider.GetConnection())
            {
                using (IQuery query = client.CreateQuery("SELECT * FROM `badge_definitions`;"))
                {
                    using (var reader = await query.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string code = reader.GetData<string>("code").ToUpper();

                            if (!badges.ContainsKey(code))
                                badges.Add(code, new BadgeDefinition(code, reader.GetData<string>("required_right")));
                        }
                    }
                }
            }

            return badges;
        }
    }
}
