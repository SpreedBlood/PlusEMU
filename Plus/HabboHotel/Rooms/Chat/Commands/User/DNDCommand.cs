namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DNDCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_dnd"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to chose the option to enable or disable console messages."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.AllowConsoleMessages = !Session.Habbo.AllowConsoleMessages;
            Session.SendWhisper("You're " + (Session.Habbo.AllowConsoleMessages == true ? "now" : "no longer") + " accepting console messages.");
        }
    }
}