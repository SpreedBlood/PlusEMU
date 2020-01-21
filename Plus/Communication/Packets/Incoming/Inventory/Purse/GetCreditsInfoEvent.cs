using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.Packets.Incoming.Inventory.Purse
{
    class GetCreditsInfoEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetCreditsInfoMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new CreditBalanceComposer(session.Habbo.Credits));
            session.SendPacket(new ActivityPointsComposer(session.Habbo.Duckets, session.Habbo.Diamonds, session.Habbo.GOTWPoints));
        }
    }
}
