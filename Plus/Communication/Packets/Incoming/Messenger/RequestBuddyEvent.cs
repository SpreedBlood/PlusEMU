using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Quests;

namespace Plus.Communication.Packets.Incoming.Messenger
{
    class RequestBuddyEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.RequestBuddyMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || session.Habbo.GetMessenger() == null)
                return;

            if (session.Habbo.GetMessenger().RequestBuddy(packet.PopString()))
                PlusEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialFriend);
        }
    }
}
