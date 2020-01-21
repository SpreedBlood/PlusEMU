using System;
using System.Text;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StatsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_stats"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "View your current statistics."; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            double Minutes = Session.Habbo.GetStats().OnlineTime / 60;
            double Hours = Minutes / 60;
            int OnlineTime = Convert.ToInt32(Hours);
            string s = OnlineTime == 1 ? "" : "s";

            StringBuilder HabboInfo = new StringBuilder();
            HabboInfo.Append("Your account stats:\r\r");

            HabboInfo.Append("Currency Info:\r");
            HabboInfo.Append("Credits: " + Session.Habbo.Credits + "\r");
            HabboInfo.Append("Duckets: " + Session.Habbo.Duckets + "\r");
            HabboInfo.Append("Diamonds: " + Session.Habbo.Diamonds + "\r");
            HabboInfo.Append("Online Time: " + OnlineTime + " Hour" + s + "\r");
            HabboInfo.Append("Respects: " + Session.Habbo.GetStats().Respect + "\r");
            HabboInfo.Append("GOTW Points: " + Session.Habbo.GOTWPoints + "\r\r");


            Session.SendNotification(HabboInfo.ToString());
        }
    }
}
