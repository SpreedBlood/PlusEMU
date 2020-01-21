using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Trading;
using Plus.Communication.Packets.Outgoing.Inventory.Trading;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Trading
{
    class InitTradeEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.InitTradeMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int userId = packet.PopInt();

            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            RoomUser roomUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (roomUser == null)
                return;

            RoomUser targetUser = room.GetRoomUserManager().GetRoomUserByVirtualId(userId);
            if (targetUser == null)
                return;

            if (session.Habbo.TradingLockExpiry > 0)
            {
                if (session.Habbo.TradingLockExpiry > PlusEnvironment.GetUnixTimestamp())
                {
                    session.SendNotification("You're currently banned from trading.");
                    return;
                }

                session.Habbo.TradingLockExpiry = 0;
                session.SendNotification("Your trading ban has now expired.");

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + session.Habbo.Id + "' LIMIT 1");
                }
            }

            if (!session.Habbo.GetPermissions().HasRight("room_trade_override"))
            {
                if (room.TradeSettings == 0)
                {
                    session.SendPacket(new TradingErrorComposer(6, targetUser.GetUsername()));
                    return;
                }

                if (room.TradeSettings == 1 && room.OwnerId != session.Habbo.Id)
                {
                    session.SendPacket(new TradingErrorComposer(6, targetUser.GetUsername()));
                    return;
                }
            }

            if (roomUser.IsTrading && roomUser.TradePartner != targetUser.UserId)
            {
                session.SendPacket(new TradingErrorComposer(7, targetUser.GetUsername()));
                return;
            }

            if (targetUser.IsTrading && targetUser.TradePartner != roomUser.UserId)
            {
                session.SendPacket(new TradingErrorComposer(8, targetUser.GetUsername()));
                return;
            }

            if (!targetUser.GetClient().Habbo.AllowTradingRequests)
            {
                session.SendPacket(new TradingErrorComposer(4, targetUser.GetUsername()));
                return;
            }

            if (targetUser.GetClient().Habbo.TradingLockExpiry > 0)
            {
                session.SendPacket(new TradingErrorComposer(4, targetUser.GetUsername()));
                return;
            }

            if (!room.GetTrading().StartTrade(roomUser, targetUser, out Trade trade))
            {
                session.SendNotification("An error occured trying to start this trade");
                return;
            }

            if (targetUser.HasStatus("trd"))
                targetUser.RemoveStatus("trd");
            if (roomUser.HasStatus("trd"))
                roomUser.RemoveStatus("trd");

            targetUser.SetStatus("trd");
            targetUser.UpdateNeeded = true;
            roomUser.SetStatus("trd");
            roomUser.UpdateNeeded = true;

            trade.SendPacket(new TradingStartComposer(roomUser.UserId, targetUser.UserId));
        }
    }
}
