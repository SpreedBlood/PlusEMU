using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class SendMsgEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SendMsgMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            int userId = packet.PopInt();
            if (userId == 0 || userId == session.Habbo.Id)
                return;

            string message = PlusEnvironment.GetGame().GetChatManager().GetFilter().CheckMessage(packet.PopString());
            if (string.IsNullOrWhiteSpace(message))
                return;


            if (session.Habbo.TimeMuted > 0)
            {
                session.SendNotification("Oops, you're currently muted - you cannot send messages.");
                return;
            }

            session.Habbo.GetMessenger().SendInstantMessage(userId, message);

        }
    }
}
