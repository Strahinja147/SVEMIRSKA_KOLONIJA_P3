using SVEMIRSKA_KOLONIJA_P3.Entiteti;
using System;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Poseduje
    {
        public virtual int Id { get; protected set; } // Dodat je surogat ključ radi lakšeg mapiranja
        public virtual string? Institucija { get; set; }
        public virtual DateTime? DatumSticanja { get; set; }
        public virtual string? NivoEkspertize { get; set; }

        public virtual Specijalizacija? Specijalizacija { get; set; }
        public virtual Stanovnik? Stanovnik { get; set; }
    }
}