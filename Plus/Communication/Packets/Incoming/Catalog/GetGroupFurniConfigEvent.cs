using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    class GetGroupFurniConfigEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetGroupFurniConfigMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new GroupFurniConfigComposer(PlusEnvironment.GetGame().GetGroupManager().GetGroupsForUser(session.Habbo.Id)));
        }
    }
}
