using System.Linq;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.Hotel
{
    class ReloadRanksCommand : IRconCommand
    {
        public string Description
        {
            get { return "This command is used to reload user permissions."; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public bool TryExecute(string[] parameters)
        {
            PlusEnvironment.GetGame().GetPermissionManager().Init();

            foreach (GameClient client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.Habbo == null || client.Habbo.GetPermissions() == null)
                    continue;

                client.Habbo.GetPermissions().Init(client.Habbo);
            }

            return true;
        }
    }
}