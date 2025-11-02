namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja 
{ 
    using FluentNHibernate.Mapping;
    using SVEMIRSKA_KOLONIJA_P3.Entiteti;

    public class MedicinskaIntervencijaMap : SubclassMap<MedicinskaIntervencija>
    {
        public MedicinskaIntervencijaMap()
        {
            DiscriminatorValue("MEDICINSKA_INTERVENCIJA");
            Map(x => x.BrojPovredjenih, "BROJ_POVREDJENIH");
            Map(x => x.StepenKriticnosti, "STEPEN_KRITICNOSTI");
        }
    }
}
