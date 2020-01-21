using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    public class GetRecyclerRewardsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetRecyclerRewardsMessageEvent;

        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new RecyclerRewardsComposer());
        }
    }
}
