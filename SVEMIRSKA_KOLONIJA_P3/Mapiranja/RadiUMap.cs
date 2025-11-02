using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    public class RadiUMap : ClassMap<RadiU>
    {
        public RadiUMap()
        {
            Table("RADI_U");

            Id(x => x.Id, "RADI_U_ID").GeneratedBy.Native("SEQ_RADI_U");

            References(x => x.SektorGdeRadi, "SEKTOR_ID");
            References(x => x.Radnik, "STANOVNIK_ID");
        }
    }
}