using FluentNHibernate.Mapping;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Mapiranja
{
    class StanovnikMap : ClassMap<Stanovnik>
    {
        public StanovnikMap()
        {
            Table("STANOVNIK");

            Id(x => x.Id, "STANOVNIK_ID").GeneratedBy.Native("SEQ_STANOVNIK");

            Map(x => x.Ime, "IME");
            Map(x => x.Prezime, "PREZIME");
            Map(x => x.Pol, "POL");
            Map(x => x.DatumRodjenja, "DATUM_RODJENJA");
            Map(x => x.Nacionalnost, "NACIONALNOST");
            Map(x => x.Zanimanje, "ZANIMANJE");
            Map(x => x.GodineUKoloniji, "GODINE_U_KOLONIJI");

            References(x => x.UcesnikZadatka, "UCESNIK_ZADATKA_ID").Unique();

            // --- ISPRAVLJENE VEZE ---

            // Kada se obriše Stanovnik, njegovi kontakti se takođe brišu. OVO JE ISPRAVNO.
            HasMany(x => x.KontaktiNaZemlji)
                .KeyColumn("STANOVNIK_ID")
                .LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse();

            // Kada se obriše Stanovnik, njegove specijalizacije (veze u tabeli POSEDUJE) se takođe brišu. OVO JE ISPRAVNO.
            HasMany(x => x.Specijalizacije)
                .KeyColumn("STANOVNIK_ID")
                .LazyLoad()
                .Cascade.AllDeleteOrphan()
                .Inverse();

            // Kada se obriše Stanovnik, Sektor kojim upravlja NE SME BITI OBRISAN.
            // .Cascade.None() sprečava bilo kakvu automatsku operaciju. Baza će prijaviti grešku ako je veza aktivna.
            HasMany(x => x.SektoriKojeVodi)
                .KeyColumn("STANOVNIK_ID")
                .LazyLoad()
                .Cascade.None() // ISPRAVLJENO
                .Inverse();

            // Kada se obriše Stanovnik, Robot za koga je odgovoran NE SME BITI OBRISAN.
            HasMany(x => x.OdgovoranZaRobote)
                .KeyColumn("ODGOVORNI_STANOVNIK_ID")
                .LazyLoad()
                .Cascade.None() // ISPRAVLJENO
                .Inverse();

            // Kada se obriše Stanovnik, Robot koga je programirao NE SME BITI OBRISAN.
            HasMany(x => x.ProgramiraniRoboti)
                .KeyColumn("PROGRAMER_STANOVNIK_ID")
                .LazyLoad()
                .Cascade.None() // ISPRAVLJENO
                .Inverse();

            // HasManyToMany veze ostaju iste jer one upravljaju samo veznom tabelom.
            HasManyToMany(x => x.RadiUSektorima)
                .Table("RADI_U")
                .ParentKeyColumn("STANOVNIK_ID")
                .ChildKeyColumn("SEKTOR_ID")
                .Cascade.All();
                

            HasManyToMany(x => x.UpravljaResursima)
                .Table("UPRAVLJA_RESURSOM")
                .ParentKeyColumn("STANOVNIK_ID")
                .ChildKeyColumn("RESURS_ID")
                .Cascade.All();

            HasManyToMany(x => x.UcestvovaoUOdrzavanjima)
                .Table("UCESNIK_ODRZAVANJA")
                .ParentKeyColumn("STANOVNIK_ID")
                .ChildKeyColumn("ZAPIS_ODRZAVANJA_ID")
                .Cascade.All();
        }
    }
}