using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class KontaktNaZemlji
    {
        public virtual int Id { get; set; }
        public virtual string? Odnos { get; set; }
        public virtual string? Ime { get; set; }
        
        public virtual Stanovnik? Stanovnik { get; set; }

        // Ova lista sada sadrži objekte nove, jednostavnije klase KonZemInf
        public virtual IList<KonZemInf> KontaktInformacije { get; set; }

        public KontaktNaZemlji()
        {
            KontaktInformacije = new List<KonZemInf>();
        }
    }
}