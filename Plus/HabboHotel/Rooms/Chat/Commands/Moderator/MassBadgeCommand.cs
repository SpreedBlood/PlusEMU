using System.Linq;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MassBadgeCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mass_badge"; }
        }

        public string Parameters
        {
            get { return "%badge%"; }
        }

        public string Description
        {
            get { return "Give a badge to the entire hotel."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the code of the badge you'd like to give to the entire hotel.");
                return;
            }

            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.Habbo == null || Client.Habbo.Username == Session.Habbo.Username)
                    continue;

                if (!Client.Habbo.GetBadgeComponent().HasBadge(Params[1]))
                {
                    Client.Habbo.GetBadgeComponent().GiveBadge(Params[1], true, Client);
                    Client.SendNotification("You have just been given a badge!");
                }
                else
                    Client.SendWhisper(Session.Habbo.Username + " tried to give you a badge, but you already have it!");
            }

            Session.SendWhisper("You have successfully given every user in this hotel the " + Params[1] + " badge!");
        }
    }
}
