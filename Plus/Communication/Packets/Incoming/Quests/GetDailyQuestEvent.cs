using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.LandingView;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class GetDailyQuestEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetDailyQuestMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int usersOnline = PlusEnvironment.GetGame().GetClientManager().Count;

            session.SendPacket(new ConcurrentUsersGoalProgressComposer(usersOnline));
        }
    }
}
