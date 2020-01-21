using Plus.Database.Interfaces;



namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableGiftsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_gifts"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to disable the ability to receive gifts or to enable the ability to receive gifts."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.AllowGifts = !Session.Habbo.AllowGifts;
            Session.SendWhisper("You're " + (Session.Habbo.AllowGifts == true ? "now" : "no longer") + " accepting gifts.");

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `allow_gifts` = @AllowGifts WHERE `id` = '" + Session.Habbo.Id + "'");
                dbClient.AddParameter("AllowGifts", PlusEnvironment.BoolToEnum(Session.Habbo.AllowGifts));
                dbClient.RunQuery();
            }
        }
    }
}