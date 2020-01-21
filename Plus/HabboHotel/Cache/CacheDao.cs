using Plus.Database;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Cache.Models;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Cache
{
    internal class CacheDao
    {
        internal async Task<UserCache> GetUserCache(int userId)
        {
            UserCache userCache = null;

            using (IAsyncDbClient client = ConnectionProvider.GetConnection())
            {
                using (IQuery query = client.CreateQuery("SELECT `username`, `motto`, `look` FROM users WHERE id = @0 LIMIT 1", userId))
                {
                    using (var reader = await query.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            userCache = new UserCache(
                                userId,
                                reader.GetData<string>("username"),
                                reader.GetData<string>("motto"),
                                reader.GetData<string>("look"));
                }
            }

            return userCache;
        }
    }
}