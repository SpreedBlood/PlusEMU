using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingOfferItemEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.TradingOfferItemMessageEvent;
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

            if (tradeUser.OfferedItems.ContainsKey(item.Id))
                return;

            trade.RemoveAccepted();

            if (tradeUser.OfferedItems.Count <= 499)
            {
                int totalLtDs = tradeUser.OfferedItems.Count(x => x.Value.LimitedNo > 0);

                if (totalLtDs < 9)
                    tradeUser.OfferedItems.Add(item.Id, item);
            }

            trade.SendPacket(new TradingUpdateComposer(trade));
        }
    }
}
