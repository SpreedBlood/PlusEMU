using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Trading;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingCancelConfirmEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.TradingCancelConfirmMessageEvent;
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
                return;

            trade.EndTrade(session.Habbo.Id);
        }
    }
}
