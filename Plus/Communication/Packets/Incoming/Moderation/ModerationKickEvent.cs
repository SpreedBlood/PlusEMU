using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModerationKickEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ModerationKickMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.GetPermissions().HasRight("mod_kick"))
                return;

            int userId = packet.PopInt();
            packet.PopString(); //message

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.Habbo == null || client.Habbo.CurrentRoomId < 1 || client.Habbo.Id == session.Habbo.Id)
                return;

            if (client.Habbo.Rank >= session.Habbo.Rank)
            {
                session.SendNotification(PlusEnvironment.GetLanguageManager().TryGetValue("moderation.kick.disallowed"));
                return;
            }

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            room.GetRoomUserManager().RemoveUserFromRoom(client, true);
        }
    }
}
