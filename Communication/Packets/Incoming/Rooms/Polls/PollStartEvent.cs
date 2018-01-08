﻿using Plus.Communication.Packets.Outgoing.Rooms.Polls;

namespace Plus.Communication.Packets.Incoming.Rooms.Polls
{
    class PollStartEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new PollContentsComposer());
        }
    }
}
