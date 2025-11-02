namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Trosi
    {
        public virtual int Id { get; protected set; }
        public virtual double? DnevniProsek { get; set; }
        public virtual int? KriticnaVrednost { get; set; }
        public virtual Sektor Sektor { get; set; }
        public virtual Resurs Resurs { get; set; }
    }
}