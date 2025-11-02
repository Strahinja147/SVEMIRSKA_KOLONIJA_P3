using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class RobotMap : ClassMap<Robot>
    {
        public RobotMap()
        {
            Table("ROBOT");
            Id(x => x.Id, "ROBOT_ID").GeneratedBy.Native("SEQ_ROBOT");

            Map(x => x.Tip, "TIP");
            Map(x => x.Sifra, "SIFRA");

            References(x => x.OdgovorniStanovnik, "ODGOVORNI_STANOVNIK_ID");
            References(x => x.Programer, "PROGRAMER_STANOVNIK_ID");
            References(x => x.Sektor, "SEKTOR_ID");
            References(x => x.UcesnikZadatka, "UCESNIK_ZADATKA_ID").Unique();
        }
    }
}