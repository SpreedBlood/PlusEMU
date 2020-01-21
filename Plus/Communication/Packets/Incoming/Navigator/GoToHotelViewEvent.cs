using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GoToHotelViewMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;


            if (session.Habbo.InRoom)
            {
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room oldRoom))
                    return;

                if (oldRoom.GetRoomUserManager() != null)
                    oldRoom.GetRoomUserManager().RemoveUserFromRoom(session, true);
            }
        }
    }
}
