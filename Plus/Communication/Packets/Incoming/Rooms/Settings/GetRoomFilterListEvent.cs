using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomFilterListEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public GetRoomFilterListEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.GetRoomFilterListMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room instance = session.Habbo.CurrentRoom;
            if (instance == null)
                return;

            if (!instance.CheckRights(session))
                return;

            session.SendPacket(new GetRoomFilterListComposer(instance));
            _achievementManager.ProgressAchievement(session, "ACH_SelfModRoomFilterSeen", 1);
        }
    }
}
