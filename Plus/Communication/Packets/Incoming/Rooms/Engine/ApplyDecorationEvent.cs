using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Quests;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class ApplyDecorationEvent : IPacketEvent
    {
        private readonly AchievementManager _achievementManager;

        public ApplyDecorationEvent(AchievementManager achievementManager)
        {
            _achievementManager = achievementManager;
        }

        public int Header => ClientPacketHeader.ApplyDecorationMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            Item item = session.Habbo.GetInventoryComponent().GetItem(packet.PopInt());
            if (item == null)
                return;

            if (item.GetBaseItem() == null)
                return;

            string decorationKey = string.Empty;
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.FLOOR:
                    decorationKey = "floor";
                    break;

                case InteractionType.WALLPAPER:
                    decorationKey = "wallpaper";
                    break;

                case InteractionType.LANDSCAPE:
                    decorationKey = "landscape";
                    break;
            }

            switch (decorationKey)
            {
                case "floor":
                    room.Floor = item.ExtraData;

                    PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniDecoFloor);
                    _achievementManager.ProgressAchievement(session, "ACH_RoomDecoFloor", 1);
                    break;

                case "wallpaper":
                    room.Wallpaper = item.ExtraData;

                    PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.FurniDecoWall);
                    _achievementManager.ProgressAchievement(session, "ACH_RoomDecoWallpaper", 1);
                    break;

                case "landscape":
                    room.Landscape = item.ExtraData;

                    _achievementManager.ProgressAchievement(session, "ACH_RoomDecoLandscape", 1);
                    break;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `rooms` SET `" + decorationKey + "` = @extradata WHERE `id` = '" + room.RoomId + "' LIMIT 1");
                dbClient.AddParameter("extradata", item.ExtraData);
                dbClient.RunQuery();

                dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + item.Id + "' LIMIT 1");
            }

            session.Habbo.GetInventoryComponent().RemoveItem(item.Id);
            room.SendPacket(new RoomPropertyComposer(decorationKey, item.ExtraData));
        }
    }
}
