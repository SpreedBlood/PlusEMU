using System;
using System.Linq;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Items;


namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    class SetMannequinFigureEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.SetMannequinFigureMessageEvent;
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
        {
            Room room = session.Habbo.CurrentRoom;
            if (room == null || !room.CheckRights(session, true))
                return;

            int itemId = packet.PopInt();
            Item item = session.Habbo.CurrentRoom.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            string gender = session.Habbo.Gender.ToLower();
            string figure = session.Habbo.Look.Split('.').Where(str => !str.Contains("hr") && !str.Contains("hd") && !str.Contains("he") && !str.Contains("ea") && !str.Contains("ha")).Aggregate("", (current, str) => current + (str + "."));

            figure = figure.TrimEnd('.');
            if (item.ExtraData.Contains(Convert.ToChar(5)))
            {
                string[] flags = item.ExtraData.Split(Convert.ToChar(5));
                item.ExtraData = gender + Convert.ToChar(5) + figure + Convert.ToChar(5) + flags[2];
            }
            else
                item.ExtraData = gender + Convert.ToChar(5) + figure + Convert.ToChar(5) + "Default";

            item.UpdateState(true, true);
        }
    }
}
