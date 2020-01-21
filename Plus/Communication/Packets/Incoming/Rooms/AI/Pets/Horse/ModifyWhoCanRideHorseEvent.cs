using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.ModifyWhoCanRideHorseMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            int petId = packet.PopInt();

            if (!room.GetRoomUserManager().TryGetPet(petId, out RoomUser pet))
                return;

            pet.PetData.AnyoneCanRide = pet.PetData.AnyoneCanRide == 1 ? 0 : 1;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `bots_petdata` SET `anyone_ride` = '" + pet.PetData.AnyoneCanRide + "' WHERE `id` = '" + petId + "' LIMIT 1");
            }

            room.SendPacket(new PetInformationComposer(pet.PetData));
        }
    }
}
