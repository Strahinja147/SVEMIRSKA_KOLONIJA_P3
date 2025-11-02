// Fajl: Mapiranja/KonZemInfMap.cs
using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class KonZemInfMap : ClassMap<KonZemInf>
    {
        public KonZemInfMap()
        {
            Table("KON_ZEM_INF");
            Id(x => x.Id, "KON_ZEM_INF_ID").GeneratedBy.Native("SEQ_KON_ZEM_INF");

            Map(x => x.KontaktInformacija, "KONTAKT_INFORMACIJE");
            References(x => x.PripadaKontaktu, "KONTAKT_ID");
        }
    }
}