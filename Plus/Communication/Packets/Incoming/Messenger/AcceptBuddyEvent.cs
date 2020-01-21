using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class AcceptBuddyEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.AcceptBuddyMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            int amount = packet.PopInt();
            if (amount > 50)
                amount = 50;
            else if (amount < 0)
                return;

            for (int i = 0; i < amount; i++)
            {
                int requestId = packet.PopInt();

                if (!session.Habbo.GetMessenger().TryGetRequest(requestId, out MessengerRequest request))
                    continue;

                if (request.To != session.Habbo.Id)
                    return;

                if (!session.Habbo.GetMessenger().FriendshipExists(request.To))
                    session.Habbo.GetMessenger().CreateFriendship(request.From);

                session.Habbo.GetMessenger().HandleRequest(requestId);
            }
        }
    }
}
