using Plus.Database.Interfaces;



namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableMimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_mimic"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to disable the ability to be mimiced or to enable the ability to be mimiced."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.AllowMimic = !Session.Habbo.AllowMimic;
            Session.SendWhisper("You're " + (Session.Habbo.AllowMimic == true ? "now" : "no longer") + " able to be mimiced.");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_mimic` = @AllowMimic WHERE `id` = '" + Session.Habbo.Id + "'");
                dbClient.AddParameter("AllowMimic", PlusEnvironment.BoolToEnum(Session.Habbo.AllowMimic));
                dbClient.RunQuery();
            }
        }
    }
}