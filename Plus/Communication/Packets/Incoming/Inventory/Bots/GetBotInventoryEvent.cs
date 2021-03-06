using System.Collections.Generic;

using Plus.HabboHotel.Users.Inventory.Bots;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Bots
{
    class GetBotInventoryEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetBotInventoryMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.Habbo.GetInventoryComponent() == null)
                return;

            ICollection<Bot> bots = session.Habbo.GetInventoryComponent().GetBots();
            session.SendPacket(new BotInventoryComposer(bots));
        }
    }
}
