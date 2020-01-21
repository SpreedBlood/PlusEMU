using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    class ModeratorActionEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ModeratorActionMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.GetPermissions().HasRight("mod_caution"))
                return;

            if (!session.Habbo.InRoom)
                return;

            Room currentRoom = session.Habbo.CurrentRoom;
            if (currentRoom == null)
                return;

            int alertMode = packet.PopInt();
            string alertMessage = packet.PopString();
            bool isCaution = alertMode != 3;

            alertMessage = isCaution ? "Caution from Moderator:\n\n" + alertMessage : "Message from Moderator:\n\n" + alertMessage;
            session.Habbo.CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
        }
    }
}
