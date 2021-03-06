using Plus.Communication.Packets.Outgoing.Avatar;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Avatar
{
    class GetWardrobeEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetWardrobeMessageEvent;

        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new WardrobeComposer(session.Habbo.Id));
        }
    }
}
