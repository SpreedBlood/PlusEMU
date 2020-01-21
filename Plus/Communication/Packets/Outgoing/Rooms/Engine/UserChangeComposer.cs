using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            WriteInteger((Self) ? -1 : User.VirtualId);
            WriteString(User.GetClient().Habbo.Look);
            WriteString(User.GetClient().Habbo.Gender);
            WriteString(User.GetClient().Habbo.Motto);
            WriteInteger(User.GetClient().Habbo.GetStats().AchievementPoints);
        }
    }
}