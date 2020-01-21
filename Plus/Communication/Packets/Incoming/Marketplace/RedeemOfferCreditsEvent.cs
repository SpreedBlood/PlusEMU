using System;
using System.Data;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;


namespace Plus.Communication.Packets.Incoming.Marketplace
{
    class RedeemOfferCreditsEvent : IPacketEvent
    {
        public int Header => ClientPacketHeader.RedeemOfferCreditsMessageEvent;
        public void Parse(GameClient session, ClientPacket packet)
        {
            int creditsOwed = 0;

            DataTable table;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `asking_price` FROM `catalog_marketplace_offers` WHERE `user_id` = '" + session.Habbo.Id + "' AND `state` = '2'");
                table = dbClient.GetTable();
            }

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    creditsOwed += Convert.ToInt32(row["asking_price"]);
                }

                if (creditsOwed >= 1)
                {
                    session.Habbo.Credits += creditsOwed;
                    session.SendPacket(new CreditBalanceComposer(session.Habbo.Credits));
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("DELETE FROM `catalog_marketplace_offers` WHERE `user_id` = '" + session.Habbo.Id + "' AND `state` = '2'");
                }
            }
        }
    }
}
