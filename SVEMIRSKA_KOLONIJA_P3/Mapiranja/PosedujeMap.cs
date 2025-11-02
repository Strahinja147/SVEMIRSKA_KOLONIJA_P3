// Fajl: Mapiranja/PosedujeMap.cs
using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class PosedujeMap : ClassMap<Poseduje>
    {
        public PosedujeMap()
        {
            // ISPRAVLJENO: Ime tabele sa velikim slovima
            Table("POSEDUJE");

            Id(x => x.Id, "POSEDuje_ID").GeneratedBy.Native("SEQ_POSEDUJE");

            Map(x => x.Institucija, "INSTITUCIJA");
            Map(x => x.DatumSticanja, "DATUM_STICANJA");
            Map(x => x.NivoEkspertize, "NIVO_EKSPERTIZE");

            References(x => x.Specijalizacija, "SPECIJALIZACIJA_ID");
            References(x => x.Stanovnik, "STANOVNIK_ID");
        }
    }
}