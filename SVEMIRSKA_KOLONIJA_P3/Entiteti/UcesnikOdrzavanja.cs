namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public class UcesnikOdrzavanja
    {
        public virtual int Id { get; protected set; }
        public virtual Stanovnik Ucesnik { get; set; }
        public virtual ZapisOdrzavanja Odrzavanje { get; set; }
    }
}