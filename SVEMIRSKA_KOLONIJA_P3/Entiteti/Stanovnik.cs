
using System;
using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Stanovnik
    {
        public virtual int Id { get; protected set; }
        public virtual string? Ime { get; set; }
        public virtual string? Prezime { get; set; }
        public virtual char Pol { get; set; }
        public virtual DateTime? DatumRodjenja { get; set; }
        public virtual string? Nacionalnost { get; set; }
        public virtual string? Zanimanje { get; set; }
        public virtual int? GodineUKoloniji { get; set; }

        public virtual UcesnikZadatka? UcesnikZadatka { get; set; }

        public virtual IList<KontaktNaZemlji> KontaktiNaZemlji { get; set; }
        public virtual IList<Poseduje> Specijalizacije { get; set; }
        public virtual IList<Sektor> SektoriKojeVodi { get; set; }
        public virtual IList<Robot> OdgovoranZaRobote { get; set; }
        public virtual IList<Robot> ProgramiraniRoboti { get; set; }
        public virtual IList<Resurs> UpravljaResursima { get; set; }
        public virtual IList<ZapisOdrzavanja> UcestvovaoUOdrzavanjima { get; set; }
        public virtual IList<Sektor> RadiUSektorima { get; set; }

        public Stanovnik()
        {
            KontaktiNaZemlji = new List<KontaktNaZemlji>();
            Specijalizacije = new List<Poseduje>();
            SektoriKojeVodi = new List<Sektor>();
            OdgovoranZaRobote = new List<Robot>();
            ProgramiraniRoboti = new List<Robot>();
            UpravljaResursima = new List<Resurs>();
            UcestvovaoUOdrzavanjima = new List<ZapisOdrzavanja>();
            RadiUSektorima = new List<Sektor>();
        }
    }
}