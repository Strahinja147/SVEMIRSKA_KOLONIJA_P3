namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Potrebna
    {
        public virtual int Id { get; protected set; }
        public virtual string Nivo { get; set; }

        public virtual Specijalizacija Specijalizacija { get; set; }
        public virtual Zadatak Zadatak { get; set; }
    }
}