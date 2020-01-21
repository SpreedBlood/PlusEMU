using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class KickUserEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public KickUserEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.KickUserMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            if (!room.CheckRights(session) && room.WhoCanKick != 2 && room.Group == null)
                return;

            if (room.Group != null && !room.CheckRights(session, false, true))
                return;

            int userId = packet.PopInt();
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
            if (user == null || user.IsBot)
                return;

            //Cannot kick owner or moderators.
            if (room.CheckRights(user.GetClient(), true) || user.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            room.GetRoomUserManager().RemoveUserFromRoom(user.GetClient(), true, true);
            _achievementManager.ProgressAchievement(session, "ACH_SelfModKickSeen", 1);
        }
    }
}
