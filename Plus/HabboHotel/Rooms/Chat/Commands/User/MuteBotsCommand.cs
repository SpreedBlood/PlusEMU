using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MuteBotsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mute_bots"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Ignore bot chat or enable it again."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.AllowBotSpeech = !Session.Habbo.AllowBotSpeech;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `bots_muted` = '" + ((Session.Habbo.AllowBotSpeech) ? 1 : 0) + "' WHERE `id` = '" + Session.Habbo.Id + "' LIMIT 1");
            }

            if (Session.Habbo.AllowBotSpeech)
                Session.SendWhisper("Change successful, you can no longer see speech from bots.");
            else
                Session.SendWhisper("Change successful, you can now see speech from bots.");
        }
    }
}
