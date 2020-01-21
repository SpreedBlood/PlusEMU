using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class MuteUserEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public MuteUserEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.MuteUserMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            int userId = packet.PopInt();
            packet.PopInt(); //roomId
            int time = packet.PopInt();

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            if (((room.WhoCanMute == 0 && !room.CheckRights(session, true) && room.Group == null) || (room.WhoCanMute == 1 && !room.CheckRights(session)) && room.Group == null) || (room.Group != null && !room.CheckRights(session, false, true)))
                return;

            RoomUser target = room.GetRoomUserManager().GetRoomUserByHabbo(PlusEnvironment.GetUsernameById(userId));
            if (target == null)
                return;
            else if (target.GetClient().Habbo.GetPermissions().HasRight("mod_tool"))
                return;

            if (room.MutedUsers.ContainsKey(userId))
            {
                if (room.MutedUsers[userId] < PlusEnvironment.GetUnixTimestamp())
                    room.MutedUsers.Remove(userId);
                else
                    return;
            }

            room.MutedUsers.Add(userId, (PlusEnvironment.GetUnixTimestamp() + (time * 60)));

            target.GetClient().SendWhisper("The room owner has muted you for " + time + " minutes!");
            _achievementManager.ProgressAchievement(session, "ACH_SelfModMuteSeen", 1);
        }
    }
}
