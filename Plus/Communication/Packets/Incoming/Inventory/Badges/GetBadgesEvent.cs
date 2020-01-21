using Plus.Communication.Packets.Outgoing.Inventory.Badges;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Badges
{
    class GetBadgesEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetBadgesMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new BadgesComposer(session));
        }
    }
}
