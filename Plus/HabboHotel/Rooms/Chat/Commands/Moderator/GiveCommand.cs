using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_give"; }
        }

        public string Parameters
        {
            get { return "%username% %type% %amount%"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter a currency type! (coins, duckets, diamonds, gotw)");
                return;
            }

            GameClient Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (Target == null)
            {
                Session.SendWhisper("Oops, couldn't find that user!");
                return;
            }

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.Habbo.GetPermissions().HasCommand("command_give_coins"))
                        {
                            Session.SendWhisper("Oops, it appears that you do not have the permissions to use this command!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.Habbo.Credits = Target.Habbo.Credits += Amount;
                                Target.SendPacket(new CreditBalanceComposer(Target.Habbo.Credits));

                                if (Target.Habbo.Id != Session.Habbo.Id)
                                    Target.SendNotification(Session.Habbo.Username + " has given you " + Amount.ToString() + " Credit(s)!");
                                Session.SendWhisper("Successfully given " + Amount + " Credit(s) to " + Target.Habbo.Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, that appears to be an invalid amount!");
                                break;
                            }
                        }
                    }

                case "pixels":
                case "duckets":
                    {
                        if (!Session.Habbo.GetPermissions().HasCommand("command_give_pixels"))
                        {
                            Session.SendWhisper("Oops, it appears that you do not have the permissions to use this command!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.Habbo.Duckets += Amount;
                                Target.SendPacket(new HabboActivityPointNotificationComposer(Target.Habbo.Duckets, Amount));

                                if (Target.Habbo.Id != Session.Habbo.Id)
                                    Target.SendNotification(Session.Habbo.Username + " has given you " + Amount.ToString() + " Ducket(s)!");
                                Session.SendWhisper("Successfully given " + Amount + " Ducket(s) to " + Target.Habbo.Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, that appears to be an invalid amount!");
                                break;
                            }
                        }
                    }

                case "diamonds":
                    {
                        if (!Session.Habbo.GetPermissions().HasCommand("command_give_diamonds"))
                        {
                            Session.SendWhisper("Oops, it appears that you do not have the permissions to use this command!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.Habbo.Diamonds += Amount;
                                Target.SendPacket(new HabboActivityPointNotificationComposer(Target.Habbo.Diamonds, Amount, 5));

                                if (Target.Habbo.Id != Session.Habbo.Id)
                                    Target.SendNotification(Session.Habbo.Username + " has given you " + Amount.ToString() + " Diamond(s)!");
                                Session.SendWhisper("Successfully given " + Amount + " Diamond(s) to " + Target.Habbo.Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, that appears to be an invalid amount!");
                                break;
                            }
                        }
                    }

                case "gotw":
                case "gotwpoints":
                    {
                        if (!Session.Habbo.GetPermissions().HasCommand("command_give_gotw"))
                        {
                            Session.SendWhisper("Oops, it appears that you do not have the permissions to use this command!");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                Target.Habbo.GOTWPoints = Target.Habbo.GOTWPoints + Amount;
                                Target.SendPacket(new HabboActivityPointNotificationComposer(Target.Habbo.GOTWPoints, Amount, 103));

                                if (Target.Habbo.Id != Session.Habbo.Id)
                                    Target.SendNotification(Session.Habbo.Username + " has given you " + Amount.ToString() + " GOTW Point(s)!");
                                Session.SendWhisper("Successfully given " + Amount + " GOTW point(s) to " + Target.Habbo.Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Oops, that appears to be an invalid amount!");
                                break;
                            }
                        }
                    }
                default:
                    Session.SendWhisper("'" + UpdateVal + "' is not a valid currency!");
                    break;
            }
        }
    }
}