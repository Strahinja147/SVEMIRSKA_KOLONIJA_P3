using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class PotrebnaMap : ClassMap<Potrebna>
    {
        public PotrebnaMap()
        {
            Table("POTREBNA");
            Id(x => x.Id, "POTREBNA_ID").GeneratedBy.Native("SEQ_POTREBNA");

            Map(x => x.Nivo, "NIVO");

            References(x => x.Specijalizacija, "SPECIJALIZACIJA_ID");
            References(x => x.Zadatak, "ZADATAK_ID");
        }
    }
}