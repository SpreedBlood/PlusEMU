using Plus.HabboHotel.Quests;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Quests;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Quests
{
    class GetCurrentQuestEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetCurrentQuestMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            Quest userQuest = PlusEnvironment.GetGame().GetQuestManager().GetQuest(session.Habbo.QuestLastCompleted);
            Quest nextQuest = PlusEnvironment.GetGame().GetQuestManager().GetNextQuestInSeries(userQuest.Category, userQuest.Number + 1);

            if (nextQuest == null)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("REPLACE INTO `user_quests`(`user_id`,`quest_id`) VALUES (" + session.Habbo.Id + ", " + nextQuest.Id + ")");
                dbClient.RunQuery("UPDATE `user_stats` SET `quest_id` = '" + nextQuest.Id + "' WHERE `id` = '" + session.Habbo.Id + "' LIMIT 1");
            }

            session.Habbo.GetStats().QuestId = nextQuest.Id;
            PlusEnvironment.GetGame().GetQuestManager().GetList(session, null);
            session.SendPacket(new QuestStartedComposer(session, nextQuest));
        }
    }
}
