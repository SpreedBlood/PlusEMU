using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Items.Wired;
using Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.Wired
{
    class SaveWiredConfigEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SaveWiredTriggeRconfigMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            if (!session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null || !room.CheckRights(session, false, true))
                return;

            int itemId = packet.PopInt();

            session.SendPacket(new HideWiredConfigComposer());

            Item selectedItem = room.GetRoomItemHandler().GetItem(itemId);
            if (selectedItem == null)
                return;

            if (!session.Habbo.CurrentRoom.GetWired().TryGet(itemId, out IWiredItem box))
                return;

            if (box.Type == WiredBoxType.EffectGiveUserBadge && !session.Habbo.GetPermissions().HasRight("room_item_wired_rewards"))
            {
                session.SendNotification("You don't have the correct permissions to do this.");
                return;
            }

            box.HandleSave(packet);
            session.Habbo.CurrentRoom.GetWired().SaveBox(box);
        }
    }
}
