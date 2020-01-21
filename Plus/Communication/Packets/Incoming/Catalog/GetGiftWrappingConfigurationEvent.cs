using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetGiftWrappingConfigurationEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetGiftWrappingConfigurationMessageEvent;

        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new GiftWrappingConfigurationComposer());
        }
    }
}
