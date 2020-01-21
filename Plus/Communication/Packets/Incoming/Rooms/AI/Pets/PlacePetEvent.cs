using System.Collections.Generic;

using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Pets;

using Plus.HabboHotel.Rooms.AI.Speech;
using log4net;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using System;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.AI.Pets
{
    class PlacePetEvent : IPacketEvent
    {
        private static readonly ILog Log = LogManager.GetLogger("Plus.Communication.Packets.Incoming.Rooms.AI.Pets.PlacePetEvent");
        public int Header => ClientPacketHeader.PlacePetMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.Habbo.InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.Habbo.CurrentRoomId, out Room room))
                return;

            if (room.AllowPets == 0 && !room.CheckRights(session, true) || !room.CheckRights(session, true))
            {
                session.SendPacket(new RoomErrorNotifComposer(1));
                return;
            }

            if (room.GetRoomUserManager().PetCount > Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("room.pets.placement_limit")))
            {
                session.SendPacket(new RoomErrorNotifComposer(2));//5 = I have too many.
                return;
            }

            if (!session.Habbo.GetInventoryComponent().TryGetPet(packet.PopInt(), out Pet pet))
                return;

            if (pet == null)
                return;

            if (pet.PlacedInRoom)
            {
                session.SendNotification("This pet is already in the room?");
                return;
            }

            int x = packet.PopInt();
            int y = packet.PopInt();

            if (!room.GetGameMap().CanWalk(x, y, false))
            {
                session.SendPacket(new RoomErrorNotifComposer(4));
                return;
            }

            if (room.GetRoomUserManager().TryGetPet(pet.PetId, out RoomUser oldPet))
            {
                room.GetRoomUserManager().RemoveBot(oldPet.VirtualId, false);
            }

            pet.X = x;
            pet.Y = y;

            pet.PlacedInRoom = true;
            pet.RoomId = room.RoomId;

            List<RandomSpeech> rndSpeechList = new List<RandomSpeech>();
            RoomBot roomBot = new RoomBot(pet.PetId, pet.RoomId, "pet", "freeroam", pet.Name, "", pet.Look, x, y, 0, 0, 0, 0, 0, 0, ref rndSpeechList, "", 0, pet.OwnerId, false, 0, false, 0);

            room.GetRoomUserManager().DeployBot(roomBot, pet);

            pet.DBState = PetDatabaseUpdateState.NeedsUpdate;
            room.GetRoomUserManager().UpdatePets();

            if (!session.Habbo.GetInventoryComponent().TryRemovePet(pet.PetId, out Pet toRemove))
            {
                Log.Error("Error whilst removing pet: " + toRemove.PetId);
                return;
            }

            session.SendPacket(new PetInventoryComposer(session.Habbo.GetInventoryComponent().GetPets()));
        }
    }
}
