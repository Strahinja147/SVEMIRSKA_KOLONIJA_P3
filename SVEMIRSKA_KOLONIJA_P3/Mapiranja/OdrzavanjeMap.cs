using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    public class OdrzavanjeMap : SubclassMap<Odrzavanje>
    {
        public OdrzavanjeMap()
        {
            DiscriminatorValue("ODRZAVANJE");
        }
    }
}