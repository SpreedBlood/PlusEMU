using System;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

using Plus.Communication.Packets.Outgoing.Rooms.Furni;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class UpdateMagicTileEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.UpdateMagicTileMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            if (!room.CheckRights(session, false, true) && !session.Habbo.GetPermissions().HasRight("room_item_use_any_stack_tile"))
                return;

            int itemId = packet.PopInt();
            int decimalHeight = packet.PopInt();

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            item.GetZ = decimalHeight / 100.0;

            room.SendPacket(new ObjectUpdateComposer(item, Convert.ToInt32(session.Habbo.Id)));
            room.SendPacket(new UpdateMagicTileComposer(itemId, decimalHeight));
        }
    }
}
