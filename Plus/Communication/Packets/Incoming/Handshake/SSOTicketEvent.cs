using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    internal class SsoTicketEvent : IPacketEvent
    {
        private readonly AuthenticationHandler _authenticationHandler;

        public SsoTicketEvent(AuthenticationHandler authenticationHandler)
        {
            _authenticationHandler = authenticationHandler;
        }

        public int Header => ClientPacketHeader.SSOTicketMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Rc4Client == null || session.Habbo != null)
                return;

            string sso = packet.PopString();
            if (string.IsNullOrEmpty(sso) || sso.Length < 3)
                return;

            _authenticationHandler.TryAuthenticate(sso, session);
        }
    }
}
