using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    class ScrGetUserInfoEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ScrGetUserInfoMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new ScrSendUserInfoComposer());
        }
    }
}
