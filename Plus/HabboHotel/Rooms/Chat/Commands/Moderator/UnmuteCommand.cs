using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UnmuteCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_unmute"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Unmute a currently muted user."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username of the user you would like to unmute.");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null || TargetClient.Habbo == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `time_muted` = '0' WHERE `id` = '" + TargetClient.Habbo.Id + "' LIMIT 1");
            }

            TargetClient.Habbo.TimeMuted = 0;
            TargetClient.SendNotification("You have been un-muted by " + Session.Habbo.Username + "!");
            Session.SendWhisper("You have successfully un-muted " + TargetClient.Habbo.Username + "!");
        }
    }
}