using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Avatar
{
    class DropHandItemEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.DropHandItemMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (user == null)
                return;

            if (user.CarryItemId > 0 && user.CarryTimer > 0)
                user.CarryItem(0);
        }
    }
}
