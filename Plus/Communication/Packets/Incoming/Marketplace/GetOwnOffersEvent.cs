using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class GetOwnOffersEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetOwnOffersMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new MarketPlaceOwnOffersComposer(session.GetHabbo().Id));
        }
    }
}
