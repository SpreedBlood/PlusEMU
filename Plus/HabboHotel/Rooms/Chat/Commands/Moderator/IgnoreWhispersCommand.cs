namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IgnoreWhispersCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_ignore_whispers"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to ignore all of the whispers in the room, except from your own."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.Habbo.IgnorePublicWhispers = !Session.Habbo.IgnorePublicWhispers;
            Session.SendWhisper("You're " + (Session.Habbo.IgnorePublicWhispers ? "now" : "no longer") + " ignoring public whispers!");
        }
    }
}
