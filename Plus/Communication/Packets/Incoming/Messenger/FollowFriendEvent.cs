using Plus.HabboHotel.GameClients;

using Plus.Communication.Packets.Outgoing.Messenger;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class FollowFriendEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.FollowFriendMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            int buddyId = packet.PopInt();
            if (buddyId == 0 || buddyId == session.Habbo.Id)
                return;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(buddyId);
            if (client == null || client.Habbo == null)
                return;

            if (!client.Habbo.InRoom)
            {
                session.SendPacket(new FollowFriendFailedComposer(2));
                session.Habbo.GetMessenger().UpdateFriend(client.Habbo.Id, client, true);
                return;
            }
            else if (session.Habbo.CurrentRoom != null && client.Habbo.CurrentRoom != null)
            {
                if (session.Habbo.CurrentRoom.RoomId == client.Habbo.CurrentRoom.RoomId)
                    return;
            }

            session.SendPacket(new RoomForwardComposer(client.Habbo.CurrentRoomId));
        }
    }
}
