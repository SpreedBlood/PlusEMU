using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;


namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class UseWallItemEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.UseWallItemMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            bool hasRights = room.CheckRights(session, false, true);

            int request = packet.PopInt();

            item.Interactor.OnTrigger(session, item, request, hasRights);
            item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, session.Habbo, item);

            PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.ExploreFindItem, item.GetBaseItem().Id);
        }
    }
}
