﻿using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    class ReloadUserRankCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload a users rank and permissions."; }
        }

        public string Parameters
        {
            get { return "%userId%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.Habbo == null)
                return false;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `rank` FROM `users` WHERE `id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", userId);
                client.Habbo.Rank = dbClient.GetInteger();
            }

            client.Habbo.GetPermissions().Init(client.Habbo);

            if (client.Habbo.GetPermissions().HasRight("mod_tickets"))
            {
                client.SendPacket(new ModeratorInitComposer(
                  PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                  PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                  PlusEnvironment.GetGame().GetModerationManager().GetTickets));
            }
            return true;
        }
    }
}