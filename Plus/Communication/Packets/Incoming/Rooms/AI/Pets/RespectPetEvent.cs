using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Pets;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class RespectPetEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public RespectPetEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.RespectPetMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom || session.Habbo.GetStats() == null || session.Habbo.GetStats().DailyPetRespectPoints == 0)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            RoomUser thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (thisUser == null)
                return;

            int petId = packet.PopInt();

            if (!session.Habbo.CurrentRoom.GetRoomUserManager().TryGetPet(petId, out RoomUser pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser targetUser = session.Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(petId);
                if (targetUser == null)
                    return;

                //Check some values first, please!
                if (targetUser.GetClient() == null || targetUser.GetClient().Habbo == null)
                    return;

                if (targetUser.GetClient().Habbo.Id == session.Habbo.Id)
                {
                    session.SendWhisper("Oops, you cannot use this on yourself! (You haven't lost a point, simply reload!)");
                    return;
                }

                //And boom! Let us send some respect points.
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialRespect);
                _achievementManager.ProgressAchievement(session, "ACH_RespectGiven", 1);
                _achievementManager.ProgressAchievement(targetUser.GetClient(), "ACH_RespectEarned", 1);

                //Take away from pet respect points, just in-case users abuse this..
                session.Habbo.GetStats().DailyPetRespectPoints -= 1;
                session.Habbo.GetStats().RespectGiven += 1;
                targetUser.GetClient().Habbo.GetStats().Respect += 1;

                //Apply the effect.
                thisUser.CarryItemId = 999999999;
                thisUser.CarryTimer = 5;

                //Send the magic out.
                if (room.RespectNotificationsEnabled)
                    room.SendPacket(new RespectPetNotificationMessageComposer(targetUser.GetClient().Habbo, targetUser));
                room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
                return;
            }

            if (pet == null || pet.PetData == null || pet.RoomId != session.Habbo.CurrentRoomId)
                return;

            session.Habbo.GetStats().DailyPetRespectPoints -= 1;
            _achievementManager.ProgressAchievement(session, "ACH_PetRespectGiver", 1);

            thisUser.CarryItemId = 999999999;
            thisUser.CarryTimer = 5;
            pet.PetData.OnRespect();
            room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
        }
    }
}
