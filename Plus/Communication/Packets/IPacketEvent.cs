using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets
{
    public interface IPacketEvent
    {
        int Header { get; }

        void Parse(GameClient session, ClientPacket packet);
    }
}