using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    public class EksperimentMap : SubclassMap<Eksperiment>
    {
        public EksperimentMap()
        {
            DiscriminatorValue("EKSPERIMENT");
            Map(x => x.NivoOpasnosti, "NIVO_OPASNOSTI");
        }
    }
}