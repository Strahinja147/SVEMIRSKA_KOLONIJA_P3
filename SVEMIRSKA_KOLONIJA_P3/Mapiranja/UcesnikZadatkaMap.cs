using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class UcesnikZadatkaMap : ClassMap<UcesnikZadatka>
    {
        public UcesnikZadatkaMap()
        {
            Table("UCESNIK_ZADATKA");
            Id(x => x.Id, "UCESNIK_ZADATKA_ID").GeneratedBy.Native("SEQ_UCESNIK_ZADATKA");

            HasOne(x => x.PripadaStanovniku).PropertyRef(nameof(Stanovnik.UcesnikZadatka));
            HasOne(x => x.PripadaRobotu).PropertyRef(nameof(Robot.UcesnikZadatka));

            HasMany(x => x.AngazovanNaZadacima)
                .KeyColumn("UCESNIK_ZADATKA_ID")
                .LazyLoad()
                .Cascade.All()
                .Inverse();
        }
    }
}