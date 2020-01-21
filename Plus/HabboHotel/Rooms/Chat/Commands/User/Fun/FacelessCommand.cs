using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class FacelessCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_faceless"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Allows you to go faceless!"; }
        }

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.Habbo.Id);
            if (User == null || User.GetClient() == null)
                return;

            string[] headParts;
            string[] figureParts = Session.Habbo.Look.Split('.');
            foreach (string Part in figureParts)
            {
                if (Part.StartsWith("hd"))
                {
                    headParts = Part.Split('-');
                    if (!headParts[1].Equals("99999"))
                        headParts[1] = "99999";
                    else
                        return;

                    Session.Habbo.Look = Session.Habbo.Look.Replace(Part, "hd-" + headParts[1] + "-" + headParts[2]);
                    break;
                }
            }

            Session.Habbo.Look = PlusEnvironment.GetFigureManager().ProcessFigure(Session.Habbo.Look, Session.Habbo.Gender, Session.Habbo.GetClothing().GetClothingParts, true);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `look` = '" + Session.Habbo.Look + "' WHERE `id` = '" + Session.Habbo.Id + "' LIMIT 1");
            }

            Session.SendPacket(new UserChangeComposer(User, true));
            Session.Habbo.CurrentRoom.SendPacket(new UserChangeComposer(User, false));
            return;
        }
    }
}
