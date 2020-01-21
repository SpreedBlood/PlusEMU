using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class SaveBrandingItemEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SaveBrandingItemMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            if (!room.CheckRights(session, true) || !session.Habbo.GetPermissions().HasRight("room_item_save_branding_items"))
                return;

            int itemId = packet.PopInt();
            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            if (item.Data.InteractionType == InteractionType.BACKGROUND)
            {
                int data = packet.PopInt();
                string brandData = "state" + Convert.ToChar(9) + "0";

                for (int i = 1; i <= data; i++)
                {
                    brandData = brandData + Convert.ToChar(9) + packet.PopString();
                }

                item.ExtraData = brandData;
            }
            else if (item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
                /*int Unknown = Packet.PopInt();
                string Data = Packet.PopString();
                int EffectId = Packet.PopInt();

                Item.ExtraData = Convert.ToString(EffectId);*/
            }

            room.GetRoomItemHandler().SetFloorItem(session, item, item.GetX, item.GetY, item.Rotation, false, false, true);
        }
    }
}
