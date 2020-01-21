using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MimicCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_mimic"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Liking someone elses swag? Copy it!"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username of the user you wish to mimic.");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (!TargetClient.Habbo.AllowMimic)
            {
                Session.SendWhisper("Oops, you cannot mimic this user - sorry!");
                return;
            }

            RoomUser TargetUser = Session.Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.Habbo.Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online or in this room.");
                return;
            }

            Session.Habbo.Gender = TargetUser.GetClient().Habbo.Gender;
            Session.Habbo.Look = TargetUser.GetClient().Habbo.Look;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `gender` = @gender, `look` = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gender", Session.Habbo.Gender);
                dbClient.AddParameter("look", Session.Habbo.Look);
                dbClient.AddParameter("id", Session.Habbo.Id);
                dbClient.RunQuery();
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.Habbo.Id);
            if (User != null)
            {
                Session.SendPacket(new AvatarAspectUpdateComposer(Session.Habbo.Look, Session.Habbo.Gender));
                Session.SendPacket(new UserChangeComposer(User, true));
                Room.SendPacket(new UserChangeComposer(User, false));
            }
        }
    }
}