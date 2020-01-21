namespace Plus.Communication.Packets.Incoming.Messenger
{
    class DeclineBuddyEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.DeclineBuddyMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            bool declineAll = packet.PopBoolean();
            packet.PopInt(); //amount

            if (!declineAll)
            {
                int requestId = packet.PopInt();
                session.Habbo.GetMessenger().HandleRequest(requestId);
            }
            else
                session.Habbo.GetMessenger().HandleAllRequests();
        }
    }
}
