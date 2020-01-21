using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.Communication.Packets.Incoming.Rooms.Chat
{
    public class StartTypingEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.StartTypingMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Username);
            if (user == null)
                return;

            session.Habbo.CurrentRoom.SendPacket(new UserTypingComposer(user.VirtualId, true));
        }
    }
}
