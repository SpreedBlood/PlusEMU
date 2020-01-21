using Plus.HabboHotel.Rooms;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class RemoveMyRightsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.RemoveMyRightsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, false))
                return;

            if (room.UsersWithRights.Contains(session.Habbo.Id))
            {
                RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
                if (user != null && !user.IsBot)
                {
                    user.RemoveStatus("flatctrl 1");
                    user.UpdateNeeded = true;

                    user.GetClient().SendPacket(new YouAreNotControllerComposer());
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `room_rights` WHERE `user_id` = @uid AND `room_id` = @rid LIMIT 1");
                    dbClient.AddParameter("uid", session.Habbo.Id);
                    dbClient.AddParameter("rid", room.Id);
                    dbClient.RunQuery();
                }

                if (room.UsersWithRights.Contains(session.Habbo.Id))
                    room.UsersWithRights.Remove(session.Habbo.Id);
            }
        }
    }
}
