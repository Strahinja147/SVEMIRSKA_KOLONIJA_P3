
using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Sektor
    {
        public virtual int Id { get; protected set; }
        public virtual string? TipSektora { get; set; }
        public virtual int? Kapacitet { get; set; }
        public virtual string? Naziv { get; set; }
        public virtual double? Povrsina { get; set; }

        public virtual Stanovnik? VodjaSektora { get; set; }

        public virtual IList<Resurs> ResursiUSektoru { get; set; }
        public virtual IList<Robot> RobotiUSektoru { get; set; }
        public virtual IList<ZapisOdrzavanja> ZapisiOdrzavanja { get; set; }
        public virtual IList<Trosi> PotrosnjaResursa { get; set; }
        public virtual IList<Stanovnik> Radnici { get; set; }

        public Sektor()
        {
            ResursiUSektoru = new List<Resurs>();
            RobotiUSektoru = new List<Robot>();
            ZapisiOdrzavanja = new List<ZapisOdrzavanja>();
            PotrosnjaResursa = new List<Trosi>();
            Radnici = new List<Stanovnik>();
        }
    }
}