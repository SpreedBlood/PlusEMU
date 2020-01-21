using Plus.HabboHotel.Users.Effects;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    class AvatarEffectActivatedEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.AvatarEffectActivatedMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int effectId = packet.PopInt();

            AvatarEffect effect = session.Habbo.Effects().GetEffectNullable(effectId, false, true);

            if (effect == null || session.Habbo.Effects().HasEffect(effectId, true))
            {
                return;
            }

            if (effect.Activate())
            {
                session.SendPacket(new AvatarEffectActivatedComposer(effect));
            }
        }
    }
}
