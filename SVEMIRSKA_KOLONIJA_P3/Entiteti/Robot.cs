namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class Robot
    {
        public virtual int Id { get; protected set; }
        public virtual string? Tip { get; set; }
        public virtual string? Sifra { get; set; }

        public virtual Stanovnik? OdgovorniStanovnik { get; set; }
        public virtual Stanovnik? Programer { get; set; }
        public virtual Sektor? Sektor { get; set; }
        public virtual UcesnikZadatka? UcesnikZadatka { get; set; }
    }
}