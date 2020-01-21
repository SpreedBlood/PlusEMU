using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DisableForcedFXCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_forced_effects"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Gives you the ability to ignore or allow forced effects."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.DisableForcedEffects = !Session.Habbo.DisableForcedEffects;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `disable_forced_effects` = @DisableForcedEffects WHERE `id` = '" + Session.Habbo.Id + "' LIMIT 1");
                dbClient.AddParameter("DisableForcedEffects", (Session.Habbo.DisableForcedEffects == true ? 1 : 0).ToString());
                dbClient.RunQuery();
            }

            Session.SendWhisper("Forced FX mode is now " + (Session.Habbo.DisableForcedEffects == true ? "disabled!" : "enabled!"));
        }
    }
}
