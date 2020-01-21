using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.Pets
{
    class GetPetInventoryEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.GetPetInventoryMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.Habbo.GetInventoryComponent() == null)
                return;

            ICollection<Pet> pets = session.Habbo.GetInventoryComponent().GetPets();
            session.SendPacket(new PetInventoryComposer(pets));
        }
    }
}
