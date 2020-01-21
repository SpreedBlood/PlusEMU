using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.Communication.Packets.Outgoing.Sound;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class SetFriendBarStateEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetFriendBarStateMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null)
                return;

            session.Habbo.FriendbarState = FriendBarStateUtility.GetEnum(packet.PopInt());
            session.SendPacket(new SoundSettingsComposer(session.Habbo.ClientVolume, session.Habbo.ChatPreference, session.Habbo.AllowMessengerInvites, session.Habbo.FocusPreference, FriendBarStateUtility.GetInt(session.Habbo.FriendbarState)));
        }
    }
}
