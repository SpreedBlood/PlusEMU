using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Handshake;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class InfoRetrieveEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.InfoRetrieveMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new UserObjectComposer(session.Habbo));
            session.SendPacket(new UserPerksComposer(session.Habbo));
        }
    }
}
