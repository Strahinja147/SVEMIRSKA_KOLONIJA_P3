using System;
using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.Entiteti
{
    public abstract class Zadatak
    {
        public virtual int Id { get; protected set; }
        public virtual int? OcekivanoTrajanjeDani { get; set; }
        public virtual DateTime? DatumZavrsetka { get; set; }
        public virtual DateTime? DatumPocetka { get; set; }
        public virtual double? OcenaUspesnosti { get; set; }
        public virtual int? BrojPotrebnihUcesnika { get; set; }
        public virtual string? Rezultat { get; set; }
        public virtual string? OpisCilja { get; set; }
        public virtual string? Lokacija { get; set; }

        public virtual Zadatak? Nadzadatak { get; set; }
        public virtual IList<Zadatak> Podzadaci { get; set; }
        public virtual IList<Potrebna> PotrebneSpecijalizacije { get; set; }
        public virtual IList<AngazovanNa> AngazovaniUcesnici { get; set; }

        public Zadatak()
        {
            Podzadaci = new List<Zadatak>();
            PotrebneSpecijalizacije = new List<Potrebna>();
            AngazovaniUcesnici = new List<AngazovanNa>();
        }
    }
}