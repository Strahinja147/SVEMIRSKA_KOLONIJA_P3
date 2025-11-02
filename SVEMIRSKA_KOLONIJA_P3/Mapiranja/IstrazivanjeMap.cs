using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    public class IstrazivanjeMap : SubclassMap<Istrazivanje>
    {
        public IstrazivanjeMap()
        {
            DiscriminatorValue("ISTRAZIVANJE");
        }
    }
}