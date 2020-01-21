using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Rooms.Connection
{
    class GoToFlatEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GoToFlatMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!session.Habbo.EnterRoom(session.Habbo.CurrentRoom))
                session.SendPacket(new CloseConnectionComposer());
        }
    }
}
