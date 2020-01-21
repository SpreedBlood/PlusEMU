using Plus.Communication.Packets.Outgoing.GameCenter;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class GetPlayableGamesEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public GetPlayableGamesEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.GetPlayableGamesMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();

            Session.SendPacket(new GameAccountStatusComposer(GameId));
            Session.SendPacket(new PlayableGamesComposer(GameId));
            Session.SendPacket(new GameAchievementListComposer(Session, _achievementManager.GetGameAchievements(GameId), GameId));
        }
    }
}
