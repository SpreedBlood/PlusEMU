using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Misc
{
    class ClientVariablesEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ClientVariablesMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            string gordanPath = packet.PopString();
            string externalVariables = packet.PopString();
        }
    }
}
