using Plus.Communication.Packets.Outgoing.Marketplace;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class GetMarketplaceCanMakeOfferEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetMarketplaceCanMakeOfferMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int errorCode = session.Habbo.TradingLockExpiry > 0 ? 6 : 1;

            session.SendPacket(new MarketplaceCanMakeOfferResultComposer(errorCode));
        }
    }
}
