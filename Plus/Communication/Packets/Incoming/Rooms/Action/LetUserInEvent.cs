using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Rooms.Action
{
    class LetUserInEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.LetUserInMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session))
                return;

            string name = packet.PopString();
            bool accepted = packet.PopBoolean();

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(name);
            if (client == null)
                return;

            if (accepted)
            {
                client.Habbo.RoomAuthOk = true;
                client.SendPacket(new FlatAccessibleComposer(""));
                room.SendPacket(new FlatAccessibleComposer(client.Habbo.Username), true);
            }
            else
            {
                client.SendPacket(new FlatAccessDeniedComposer(""));
                room.SendPacket(new FlatAccessDeniedComposer(client.Habbo.Username), true);
            }
        }
    }
}
