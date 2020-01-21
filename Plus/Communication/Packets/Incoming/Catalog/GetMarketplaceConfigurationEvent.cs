using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetMarketplaceConfigurationMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new MarketplaceConfigurationComposer());
        }
    }
}
