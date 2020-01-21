using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Users
{
    class RespectUserEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public RespectUserEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.RespectUserMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            if (!session.Habbo.InRoom || session.Habbo.GetStats().DailyRespectPoints <= 0)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(packet.PopInt());
            if (user == null || user.GetClient() == null || user.GetClient().Habbo.Id == session.Habbo.Id || user.IsBot)
                return;

            RoomUser thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (thisUser == null)
                return;

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialRespect);
            _achievementManager.ProgressAchievement(session, "ACH_RespectGiven", 1);
            _achievementManager.ProgressAchievement(user.GetClient(), "ACH_RespectEarned", 1);

            session.Habbo.GetStats().DailyRespectPoints -= 1;
            session.Habbo.GetStats().RespectGiven += 1;
            user.GetClient().Habbo.GetStats().Respect += 1;

            if (room.RespectNotificationsEnabled)
                room.SendPacket(new RespectNotificationComposer(user.GetClient().Habbo.Id, user.GetClient().Habbo.GetStats().Respect));
            room.SendPacket(new ActionComposer(thisUser.VirtualId, 7));
        }
    }
}
