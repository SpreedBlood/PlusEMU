using Plus.Database;
using Plus.Database.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Bots.Models
{
    internal class BotDao
    {
        internal async Task<List<BotResponse>> GetResponses()
        {
            List<BotResponse> responses = new List<BotResponse>();

            using (IAsyncDbClient dbclient = ConnectionProvider.GetConnection())
            {
                using (IQuery query = dbclient.CreateQuery("SELECT `bot_ai`,`chat_keywords`,`response_text`,`response_mode`,`response_beverage` FROM `bots_responses`"))
                {
                    using (var reader = await query.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            responses.Add(new BotResponse(
                                reader.GetData<string>("bot_ai"),
                                reader.GetData<string>("chat_keywords"),
                                reader.GetData<string>("response_text"),
                                reader.GetData<string>("response_mode"),
                                reader.GetData<string>("response_beverage")));
                }
            }

            return responses;
        }
    }
}