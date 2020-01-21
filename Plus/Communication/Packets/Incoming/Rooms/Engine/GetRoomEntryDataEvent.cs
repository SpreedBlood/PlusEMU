using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items.Wired;

using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine
{
    class GetRoomEntryDataEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetRoomEntryDataMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            if (session.Habbo.InRoom)
            {
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room oldRoom))
                    return;

                if (oldRoom.GetRoomUserManager() != null)
                    oldRoom.GetRoomUserManager().RemoveUserFromRoom(session, false);
            }

            if (!room.GetRoomUserManager().AddAvatarToRoom(session))
            {
                room.GetRoomUserManager().RemoveUserFromRoom(session, false);
                return;//TODO: Remove?
            }

            room.SendObjects(session);

            if (session.Habbo.GetMessenger() != null)
                session.Habbo.GetMessenger().OnStatusChanged(true);

            if (session.Habbo.GetStats().QuestId > 0)
                PlusEnvironment.GetGame().GetQuestManager().QuestReminder(session, session.Habbo.GetStats().QuestId);

            session.SendPacket(new RoomEntryInfoComposer(room.RoomId, room.CheckRights(session, true)));
            session.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.Hidewall.ToString())));

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Username);
            if (user != null && session.Habbo.PetId == 0)
            {
                room.SendPacket(new UserChangeComposer(user, false));
            }

            session.SendPacket(new RoomEventComposer(room, room.Promotion));

            if (room.GetWired() != null)
                room.GetWired().TriggerEvent(WiredBoxType.TriggerRoomEnter, session.Habbo);

            if (PlusEnvironment.GetUnixTimestamp() < session.Habbo.FloodTime && session.Habbo.FloodTime != 0)
                session.SendPacket(new FloodControlComposer((int)session.Habbo.FloodTime - (int)PlusEnvironment.GetUnixTimestamp()));
        }
    }
}
