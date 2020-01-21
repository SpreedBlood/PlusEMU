using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    class AvatarEffectSelectedEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.AvatarEffectSelectedMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int effectId = packet.PopInt();
            if (effectId < 0)
                effectId = 0;

            if (!session.Habbo.InRoom)
                return;

            Room room = session.Habbo.CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.Habbo.Id);
            if (user == null)
                return;

            if (effectId != 0 && session.Habbo.Effects().HasEffect(effectId, true))
                user.ApplyEffect(effectId);
        }
    }
}
