using Plus.Communication.Packets.Outgoing.Navigator;

using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    public class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.DeleteFavouriteRoomMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int id = packet.PopInt();

            session.Habbo.FavoriteRooms.Remove(id);
            session.SendPacket(new UpdateFavouriteRoomComposer(id, false));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = " + session.Habbo.Id + " AND room_id = " + id + " LIMIT 1");
            }
        }
    }
}
