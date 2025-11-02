namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Evakuacija : Zadatak
    {
        public virtual string? OblastEvakuacije { get; set; }
        public virtual int? BrojOsobaEvakuacije { get; set; }
    }
}