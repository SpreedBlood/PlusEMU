using Plus.Communication.Packets.Outgoing.Sound;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Sound
{
    class GetSongInfoEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetSongInfoMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new TraxSongInfoComposer());
        }
    }
}
