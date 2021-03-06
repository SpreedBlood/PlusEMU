using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingModifyEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.TradingModifyMessageEvent;
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

            if (!trade.CanChange)
                return;

            TradeUser user = trade.Users[0];

            if (user.RoomUser != roomUser)
                user = trade.Users[1];

            user.HasAccepted = false;
            trade.SendPacket(new TradingAcceptComposer(session.Habbo.Id, false));
        }
    }
}
