using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class SsoTicketEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SSOTicketMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Rc4Client == null || session.GetHabbo() != null)
                return;

            string sso = packet.PopString();
            if (string.IsNullOrEmpty(sso) || sso.Length < 3)
                return;

            session.TryAuthenticate(sso); //Spreed.
        }
    }
}
