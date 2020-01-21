using System.Collections.Generic;

using Plus.HabboHotel.Games;
using Plus.Communication.Packets.Outgoing.GameCenter;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    class GetGameListingEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetGameListingMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<GameData> Games = PlusEnvironment.GetGame().GetGameDataManager().GameData;

            Session.SendPacket(new GameListComposer(Games));
        }
    }
}
