using System.Linq;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Achievements
{
    class GetAchievementsEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public GetAchievementsEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.GetAchievementsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new AchievementsComposer(session, _achievementManager.Achievements.Values.ToList()));
        }
    }
}
