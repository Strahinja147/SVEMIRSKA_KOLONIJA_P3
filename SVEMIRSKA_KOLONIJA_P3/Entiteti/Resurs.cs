using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Resurs
    {
        public virtual int Id { get; protected set; }
        public virtual string? Naziv { get; set; }
        public virtual int? TrenutnaKolicina { get; set; }

        public virtual Sektor? Sektor { get; set; }

        public virtual IList<Trosi> PotrosnjaPoSektorima { get; set; }
   
        // IZMENA: Lista veza umesto liste stanovnika
        public virtual IList<UpravljaResursom> UpravljaVeze { get; set; }

        public Resurs()
        {
            PotrosnjaPoSektorima = new List<Trosi>();
            UpravljaVeze = new List<UpravljaResursom>(); // Inicijalizacija nove liste
        }
    }
}