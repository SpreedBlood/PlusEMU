using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class AddFavouriteRoomEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.AddFavouriteRoomMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null)
                return;

            int roomId = packet.PopInt();

            if (!RoomFactory.TryGetData(roomId, out RoomData data))
                return;

            if (data == null || session.Habbo.FavoriteRooms.Count >= 30 || session.Habbo.FavoriteRooms.Contains(roomId))
            {
                // send packet that favourites is full.
                return;
            }

            session.Habbo.FavoriteRooms.Add(roomId);
            session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + session.Habbo.Id + "," + roomId + ")");
            }
        }
    }
}
