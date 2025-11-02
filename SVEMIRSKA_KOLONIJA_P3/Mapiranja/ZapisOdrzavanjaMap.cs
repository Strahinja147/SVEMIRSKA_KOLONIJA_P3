using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class ZapisOdrzavanjaMap : ClassMap<ZapisOdrzavanja>
    {
        public ZapisOdrzavanjaMap()
        {
            Table("ZAPIS_ODRZAVANJA");
            Id(x => x.Id, "ZAPIS_ID").GeneratedBy.Native("SEQ_ZAPIS_ODRZAVANJA");
            Map(x => x.VremeOdrzavanja, "VREME_ODRZAVANJA");

            References(x => x.Sektor, "SEKTOR_ID");

            HasManyToMany(x => x.UcesniciOdrzavanja)
                .Table("UCESNIK_ODRZAVANJA")
                .ParentKeyColumn("ZAPIS_ODRZAVANJA_ID")
                .ChildKeyColumn("STANOVNIK_ID")
                .Cascade.All();
        }
    }
}