using System.Linq;
using System.Data;
using System.Collections.Generic;
using log4net;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Bots.Models;

namespace Plus.HabboHotel.Bots
{
    public class BotManager
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.HabboHotel.Bots.BotManager");

        private readonly BotDao _dao;
        private List<BotResponse> _responses;

        public BotManager()
        {
            _dao = new BotDao();
            _responses = new List<BotResponse>();
        }

        public async void Init()
        {
            if (_responses.Count > 0)
                _responses.Clear();

            _responses = await _dao.GetResponses();
        }

        public BotResponse GetResponse(BotAIType type, string message)
        {
            foreach (BotResponse response in _responses.Where(x => x.AiType == type).ToList())
            {
                if (response.KeywordMatched(message))
                {
                    return response;
                }
            }

            return null;
        }
    }
}