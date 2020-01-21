using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Session;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SummonCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_summon"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Bring another user to your current room."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter the username of the user you wish to summon.");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (TargetClient.Habbo == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            if (TargetClient.Habbo.Username == Session.Habbo.Username)
            {
                Session.SendWhisper("Get a life.");
                return;
            }

            TargetClient.SendNotification("You have been summoned to " + Session.Habbo.Username + "!");
            if (!TargetClient.Habbo.InRoom)
                TargetClient.SendPacket(new RoomForwardComposer(Session.Habbo.CurrentRoomId));
            else
                TargetClient.Habbo.PrepareRoom(Session.Habbo.CurrentRoomId, "");
        }
    }
}