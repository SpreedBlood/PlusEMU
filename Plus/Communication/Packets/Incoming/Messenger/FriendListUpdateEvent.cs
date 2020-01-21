using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FriendListUpdateEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.FriendListUpdateMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {

        }
    }
}
