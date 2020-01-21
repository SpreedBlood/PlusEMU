using System.Collections.Generic;
using log4net;
using Plus.HabboHotel.Badges.Models;

namespace Plus.HabboHotel.Badges
{
    public class BadgeManager
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.HabboHotel.Badges.BadgeManager");

        private readonly BadgeDao _dao;
        private Dictionary<string, BadgeDefinition> _badges;

        public BadgeManager()
        {
            _badges = new Dictionary<string, BadgeDefinition>();
            _dao = new BadgeDao();
        }

        public async void Init()
        {
            _badges = await _dao.GetBadges();
            Log.Info("Loaded " + _badges.Count + " badge definitions.");
        }

        public bool TryGetBadge(string code, out BadgeDefinition badge)
        {
            return _badges.TryGetValue(code.ToUpper(), out badge);
        }
    }
}