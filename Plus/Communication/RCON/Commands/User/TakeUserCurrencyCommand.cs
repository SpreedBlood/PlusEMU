using System;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.Communication.Rcon.Commands.User
{
    class TakeUserCurrencyCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to take a specified amount of a specified currency from a user."; }
        }

        public string Parameters
        {
            get { return "%userId% %currency% %amount%"; }
        }

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.Habbo == null)
                return false;

            // Validate the currency type
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string currency = Convert.ToString(parameters[1]);

            if (!int.TryParse(parameters[2], out int amount))
                return false;

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                    {
                        client.Habbo.Credits -= amount;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("credits", client.Habbo.Credits);
                            dbClient.AddParameter("id", userId);
                            dbClient.RunQuery();
                        }

                        client.SendPacket(new CreditBalanceComposer(client.Habbo.Credits));
                        break;
                    }

                case "pixels":
                case "duckets":
                    {
                        client.Habbo.Duckets -= amount;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("duckets", client.Habbo.Duckets);
                            dbClient.AddParameter("id", userId);
                            dbClient.RunQuery();
                        }

                        client.SendPacket(new HabboActivityPointNotificationComposer(client.Habbo.Duckets, amount));
                        break;
                    }

                case "diamonds":
                    {
                        client.Habbo.Diamonds -= amount;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("diamonds", client.Habbo.Diamonds);
                            dbClient.AddParameter("id", userId);
                            dbClient.RunQuery();
                        }

                        client.SendPacket(new HabboActivityPointNotificationComposer(client.Habbo.Diamonds, 0, 5));
                        break;
                    }

                case "gotw":
                    {
                        client.Habbo.GOTWPoints -= amount;

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `gotw_points` = @gotw WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("gotw", client.Habbo.GOTWPoints);
                            dbClient.AddParameter("id", userId);
                            dbClient.RunQuery();
                        }

                        client.SendPacket(new HabboActivityPointNotificationComposer(client.Habbo.GOTWPoints, 0, 103));
                        break;
                    }
            }
            return true;
        }
    }
}