using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingOfferItemsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.TradingOfferItemsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (roomUser == null)
                return;

            int amount = packet.PopInt();
            int itemId = packet.PopInt();

            if (!roomUser.IsTrading)
            {
                session.SendPacket(new TradingClosedComposer(session.Habbo.Id));
                return;
            }

            if (!room.GetTrading().TryGetTrade(roomUser.TradeId, out Trade trade))
            {
                session.SendPacket(new TradingClosedComposer(session.Habbo.Id));
                return;
            }

            Item item = session.Habbo.GetInventoryComponent().GetItem(itemId);
            if (item == null)
                return;

            if (!trade.CanChange)
                return;

            TradeUser tradeUser = trade.Users[0];

            if (tradeUser.RoomUser != roomUser)
                tradeUser = trade.Users[1];

            List<Item> allItems = session.Habbo.GetInventoryComponent().GetItems.Where(x => x.Data.Id == item.Data.Id).Take(amount).ToList();
            foreach (Item I in allItems)
            {
                if (tradeUser.OfferedItems.ContainsKey(I.Id))
                    return;

                trade.RemoveAccepted();

                tradeUser.OfferedItems.Add(I.Id, I);
            }

            trade.SendPacket(new TradingUpdateComposer(trade));
        }
    }
}
