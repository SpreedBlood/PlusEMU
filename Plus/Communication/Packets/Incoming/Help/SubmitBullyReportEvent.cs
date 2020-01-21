using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Help;

namespace Plus.Communication.Packets.Incoming.Help
{
    class SubmitBullyReportEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SubmitBullyReportMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            //0 = sent, 1 = blocked, 2 = no chat, 3 = already reported.
            if (session == null)
                return;

            int userId = packet.PopInt();
            if (userId == session.Habbo.Id)//Hax
                return;

            if (session.Habbo.AdvertisingReportedBlocked)
            {
                session.SendPacket(new SubmitBullyReportComposer(1));//This user is blocked from reporting.
                return;
            }

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Convert.ToInt32(userId));
            if (client == null)
            {
                session.SendPacket(new SubmitBullyReportComposer(0));//Just say it's sent, the user isn't found.
                return;
            }

            if (session.Habbo.LastAdvertiseReport > PlusEnvironment.GetUnixTimestamp())
            {
                session.SendNotification("Reports can only be sent per 5 minutes!");
                return;
            }

            if (client.Habbo.GetPermissions().HasRight("mod_tool"))//Reporting staff, nope!
            {
                session.SendNotification("Sorry, you cannot report staff members via this tool.");
                return;
            }

            //This user hasn't even said a word, nope!
            if (!client.Habbo.HasSpoken)
            {
                session.SendPacket(new SubmitBullyReportComposer(2));
                return;
            }

            //Already reported, nope.
            if (client.Habbo.AdvertisingReported && session.Habbo.Rank < 2)
            {
                session.SendPacket(new SubmitBullyReportComposer(3));
                return;
            }

            if (session.Habbo.Rank <= 1)
                session.Habbo.LastAdvertiseReport = PlusEnvironment.GetUnixTimestamp() + 300;
            else
                session.Habbo.LastAdvertiseReport = PlusEnvironment.GetUnixTimestamp();

            client.Habbo.AdvertisingReported = true;
            session.SendPacket(new SubmitBullyReportComposer(0));
            //PlusEnvironment.GetGame().GetClientManager().ModAlert("New advertising report! " + Client.Habbo.Username + " has been reported for advertising by " + Session.Habbo.Username +".");
            PlusEnvironment.GetGame().GetClientManager().DoAdvertisingReport(session, client);
        }
    }
}
