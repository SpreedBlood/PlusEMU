using Plus.HabboHotel.Items;
using Plus.HabboHotel.Groups;

using Plus.Communication.Packets.Outgoing.Groups;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetGroupFurniSettingsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.Habbo == null || !session.Habbo.InRoom)
                return;

            int itemId = packet.PopInt();
            int groupId = packet.PopInt();

            Item item = session.Habbo.CurrentRoom.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            if (item.Data.InteractionType != InteractionType.GUILD_GATE)
                return;

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            session.SendPacket(new GroupFurniSettingsComposer(group, itemId, session.Habbo.Id));
            session.SendPacket(new GroupInfoComposer(group, session));
        }
    }
}
