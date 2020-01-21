using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Catalog;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetClubGiftsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetClubGiftsMessageEvent;

        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new ClubGiftsComposer());
        }
    }
}
