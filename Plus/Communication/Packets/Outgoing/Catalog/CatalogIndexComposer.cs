using System.Collections.Generic;
using Plus.HabboHotel.Catalog;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CatalogIndexComposer : ServerPacket
    {
        public CatalogIndexComposer(GameClient sesion, ICollection<CatalogPage> pages)
            : base(ServerPacketHeader.CatalogIndexMessageComposer)
        {
            WriteRootIndex(sesion, pages);

            foreach (CatalogPage Parent in pages)
            {
                if (Parent.ParentId != -1 || Parent.MinimumRank > sesion.Habbo.Rank || (Parent.MinimumVIP > sesion.Habbo.VIPRank && sesion.Habbo.Rank == 1))
                    continue;

                WritePage(Parent, CalcTreeSize(sesion, pages, Parent.Id));

                foreach (CatalogPage child in pages)
                {
                    if (child.ParentId != Parent.Id || child.MinimumRank > sesion.Habbo.Rank || (child.MinimumVIP > sesion.Habbo.VIPRank && sesion.Habbo.Rank == 1))
                        continue;

                    if (child.Enabled)
                        WritePage(child, CalcTreeSize(sesion, pages, child.Id));
                    else
                        WriteNodeIndex(child, CalcTreeSize(sesion, pages, child.Id));

                    foreach (CatalogPage SubChild in pages)
                    {
                        if (SubChild.ParentId != child.Id || SubChild.MinimumRank > sesion.Habbo.Rank)
                            continue;

                        WritePage(SubChild, 0);
                    }
                }
            }

            WriteBoolean(false);
            WriteString("NORMAL");
        }

        public void WriteRootIndex(GameClient session, ICollection<CatalogPage> pages)
        {
            WriteBoolean(true);
            WriteInteger(0);
            WriteInteger(-1);
            WriteString("root");
            WriteString(string.Empty);
            WriteInteger(0);
            WriteInteger(CalcTreeSize(session, pages, -1));
        }

        public void WriteNodeIndex(CatalogPage page, int treeSize)
        {
            WriteBoolean(page.Visible);
            WriteInteger(page.Icon);
            WriteInteger(-1);
            WriteString(page.PageLink);
            WriteString(page.Caption);
            WriteInteger(0);
            WriteInteger(treeSize);
        }

        public void WritePage(CatalogPage page, int treeSize)
        {
            WriteBoolean(page.Visible);
            WriteInteger(page.Icon);
            WriteInteger(page.Id);
            WriteString(page.PageLink);
            WriteString(page.Caption);

            WriteInteger(page.ItemOffers.Count);
            foreach (int i in page.ItemOffers.Keys)
            {
                WriteInteger(i);
            }

            WriteInteger(treeSize);
        }

        public int CalcTreeSize(GameClient Session, ICollection<CatalogPage> Pages, int ParentId)
        {
            int i = 0;
            foreach (CatalogPage Page in Pages)
            {
                if (Page.MinimumRank > Session.Habbo.Rank || (Page.MinimumVIP > Session.Habbo.VIPRank && Session.Habbo.Rank == 1) || Page.ParentId != ParentId)
                    continue;

                if (Page.ParentId == ParentId)
                    i++;
            }

            return i;
        }
    }
}