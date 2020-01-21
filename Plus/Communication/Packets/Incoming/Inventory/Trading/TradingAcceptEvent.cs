using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingAcceptEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.TradingAcceptMessageEvent;
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

            if (!room.GetTrading().TryGetTrade(roomUser.TradeId, out Trade trade))
            {
                session.SendPacket(new TradingClosedComposer(session.Habbo.Id));
                return;
            }

            TradeUser tradeUser = trade.Users[0];
            if (tradeUser.RoomUser != roomUser)
                tradeUser = trade.Users[1];

            tradeUser.HasAccepted = true;
            trade.SendPacket(new TradingAcceptComposer(session.Habbo.Id, true));

            if (trade.AllAccepted)
            {
                trade.SendPacket(new TradingCompleteComposer());
                trade.CanChange = false;
                trade.RemoveAccepted();
            }
        }
    }
}
