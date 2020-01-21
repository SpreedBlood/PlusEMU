using System.Linq;
using System.Collections.Generic;

using Plus.HabboHotel.Users.Messenger;
using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class GetBuddyRequestsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetBuddyRequestsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<MessengerRequest> requests = session.Habbo.GetMessenger().GetRequests().ToList();

            session.SendPacket(new BuddyRequestsComposer(requests));
        }
    }
}
