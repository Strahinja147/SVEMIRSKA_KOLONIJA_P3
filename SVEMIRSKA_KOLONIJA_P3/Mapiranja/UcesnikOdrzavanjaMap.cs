using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    public class UcesnikOdrzavanjaMap : ClassMap<UcesnikOdrzavanja>
    {
        public UcesnikOdrzavanjaMap()
        {
            Table("UCESNIK_ODRZAVANJA");

            Id(x => x.Id, "UCESNIK_ODRZAVANJA_ID").GeneratedBy.Native("SEQ_UCESNIK_ODRZAVANJA");

            References(x => x.Ucesnik, "STANOVNIK_ID");
            References(x => x.Odrzavanje, "ZAPIS_ODRZAVANJA_ID");
        }
    }
}