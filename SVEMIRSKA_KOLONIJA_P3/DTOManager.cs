using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Proxy;
using ISession = NHibernate.ISession;
using NHibernate.Linq;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;
using SVEMIRSKA_KOLONIJA_P3.DTOs;
using SVEMIRSKA_KOLONIJA_P3; // Obavezno dodati using za namespace gde se nalazi DataLayer

namespace SVEMIRSKA_KOLONIJA
{
    public static class DTOManager
    {
        #region Stanovnik

        /// <summary>
        /// Vraća listu svih stanovnika sa osnovnim podacima. U slučaju greške, prikazuje poruku i vraća praznu listu.
        /// </summary>
        public static List<StanovnikPregled> VratiSveStanovnike()
        {
            var stanovnici = new List<StanovnikPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                IEnumerable<Stanovnik> sviStanovnici = from o in s.Query<Stanovnik>() select o;

                foreach (Stanovnik st in sviStanovnici)
                {
                    stanovnici.Add(new StanovnikPregled
                    {
                        Id = st.Id,
                        Ime = st.Ime,
                        Prezime = st.Prezime,
                        Zanimanje = st.Zanimanje
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return stanovnici;
        }

        /// <summary>
        /// Vraća sve detaljne informacije o jednom stanovniku. U slučaju greške, prikazuje poruku i vraća null.
        /// </summary>
        public static StanovnikDetalji? VratiStanovnikaDetalji(int id)
        {
            StanovnikDetalji? stanovnik = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                // 2. IZMENA: Ključna promena sa Load() na Get()
                Stanovnik? st = s.Get<Stanovnik>(id);

                // 3. IZMENA: Proveravamo da li je stanovnik pronađen
                if (st == null)
                {
                    // Ako nije, vraćamo null. Kontroler će ovo pretvoriti u 404 Not Found.
                    return null;
                }

                // Ovaj kod se izvršava samo ako je stanovnik 'st' pronađen
                stanovnik = new StanovnikDetalji
                {
                    Id = st.Id,
                    Ime = st.Ime,
                    Prezime = st.Prezime,
                    Pol = st.Pol,
                    DatumRodjenja = st.DatumRodjenja,
                    Nacionalnost = st.Nacionalnost,
                    Zanimanje = st.Zanimanje,
                    GodineUKoloniji = st.GodineUKoloniji,
                    KontaktiNaZemlji = st.KontaktiNaZemlji.Select(k => new KontaktNaZemljiPregled
                    {
                        Id = k.Id,
                        Ime = k.Ime,
                        Odnos = k.Odnos,
                        KontaktInformacije = k.KontaktInformacije.Select(info => info.KontaktInformacija).ToList()
                    }).ToList(),
                    Specijalizacije = st.Specijalizacije.Select(p => new PosedujePregled
                    {
                        Id = p.Id,
                        NazivSpecijalizacije = p.Specijalizacija?.Naziv,
                        NivoEkspertize = p.NivoEkspertize,
                        DatumSticanja = p.DatumSticanja,
                        Institucija = p.Institucija,
                        Specijalizacija = p.Specijalizacija != null ? new SpecijalizacijaPregled
                        {
                            Id = p.Specijalizacija.Id,
                            Naziv = p.Specijalizacija.Naziv
                        } : null
                    }).ToList(),
                    SektoriKojeVodi = st.SektoriKojeVodi.Select(sek => new SektorPregled
                    {
                        Id = sek.Id,
                        Naziv = sek.Naziv,
                        TipSektora = sek.TipSektora
                    }).ToList(),
                    OdgovoranZaRobote = st.OdgovoranZaRobote.Select(r => new RobotPregled
                    {
                        Id = r.Id,
                        Sifra = r.Sifra,
                        Tip = r.Tip
                    }).ToList(),
                    ProgramiraniRoboti = st.ProgramiraniRoboti.Select(r => new RobotPregled
                    {
                        Id = r.Id,
                        Sifra = r.Sifra,
                        Tip = r.Tip
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                // Catch blok je za neočekivane greške servera
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return stanovnik;
        }

        /// <summary>
        /// Dodaje novog stanovnika u bazu unutar transakcije.
        /// </summary>
        public static Stanovnik DodajStanovnika(StanovnikDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var stanovnik = new Stanovnik
                {
                    Ime = p.Ime,
                    Prezime = p.Prezime,
                    Pol = p.Pol,
                    DatumRodjenja = p.DatumRodjenja,
                    Nacionalnost = p.Nacionalnost,
                    Zanimanje = p.Zanimanje,
                    GodineUKoloniji = p.GodineUKoloniji
                };

                s.Save(stanovnik);
                t.Commit();
                return stanovnik; // VRAĆAMO SAČUVANI ENTITET SA NOVIM ID-JEM
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Ažurira postojećeg stanovnika u bazi unutar transakcije.
        /// </summary>
        public static void AzurirajStanovnika(StanovnikDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var stanovnik = s.Load<Stanovnik>(p.Id);

                stanovnik.Ime = p.Ime;
                stanovnik.Prezime = p.Prezime;
                stanovnik.Pol = p.Pol;
                stanovnik.DatumRodjenja = p.DatumRodjenja;
                stanovnik.Nacionalnost = p.Nacionalnost;
                stanovnik.Zanimanje = p.Zanimanje;
                stanovnik.GodineUKoloniji = p.GodineUKoloniji;

                s.Update(stanovnik);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše stanovnika iz baze unutar transakcije.
        /// </summary>
        public static void ObrisiStanovnika(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                Stanovnik st = s.Load<Stanovnik>(id);
                s.Delete(st); // Pusti bazu da javi grešku ako je stanovnik i dalje negde vođa ili odgovoran

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        #endregion

        // ... Ostali regioni (Sektor, Robot, itd.) bi sledili isti obrazac ...
        #region Sektor

        /// <summary>
        /// Vraća listu svih sektora sa osnovnim podacima.
        /// </summary>
        public static List<SektorPregled> VratiSveSektore()
        {
            var sektori = new List<SektorPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sviSektori = s.Query<Sektor>().ToList();

                foreach (var sekt in sviSektori)
                {
                    sektori.Add(new SektorPregled
                    {
                        Id = sekt.Id,
                        Naziv = sekt.Naziv,
                        TipSektora = sekt.TipSektora
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return sektori;
        }

        /// <summary>
        /// Vraća sve detaljne informacije o jednom sektoru.
        /// </summary>
        public static SektorDetalji? VratiSektorDetalji(int id)
        {
            SektorDetalji? sektor = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                // 1. IZMENA: Ključna promena sa Load() na Get()
                var sekt = s.Get<Sektor>(id);

                // 2. IZMENA: Proveravamo da li je sektor pronađen
                if (sekt == null)
                {
                    // Ako nije, vraćamo null. Kontroler će ovo pretvoriti u 404 Not Found.
                    return null;
                }

                // Ovaj kod se izvršava samo ako je sektor pronađen
                sektor = new SektorDetalji
                {
                    Id = sekt.Id,
                    Naziv = sekt.Naziv,
                    TipSektora = sekt.TipSektora,
                    Kapacitet = sekt.Kapacitet,
                    Povrsina = sekt.Povrsina,
                    VodjaSektora = sekt.VodjaSektora != null ? new StanovnikPregled
                    {
                        Id = sekt.VodjaSektora.Id,
                        Ime = sekt.VodjaSektora.Ime,
                        Prezime = sekt.VodjaSektora.Prezime,
                        Zanimanje = sekt.VodjaSektora.Zanimanje
                    } : null,
                    Radnici = sekt.Radnici.Select(veza => new StanovnikPregled
                    {
                        Id = veza.Id,
                        Ime = veza.Ime,
                        Prezime = veza.Prezime,
                        Zanimanje = veza.Zanimanje
                    }).ToList(),
                    ResursiUSektoru = sekt.ResursiUSektoru.Select(r => new ResursPregled
                    {
                        Id = r.Id,
                        Naziv = r.Naziv,
                        TrenutnaKolicina = r.TrenutnaKolicina
                    }).ToList(),
                    IstorijaOdrzavanja = sekt.ZapisiOdrzavanja.Select(zo => new ZapisOdrzavanjaPregled
                    {
                        Id = zo.Id,
                        VremeOdrzavanja = zo.VremeOdrzavanja,
                        NazivSektora = zo.Sektor.Naziv
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                // Catch blok je za neočekivane greške servera
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return sektor;
        }

        /// <summary>
        /// Dodaje novi sektor u bazu unutar transakcije.
        /// </summary>
        //public static Sektor DodajSektor(SektorDetalji p)
        //{
        //    ISession? s = null;
        //    ITransaction? t = null;
        //    try
        //    {
        //        s = DataLayer.GetSession();
        //        t = s.BeginTransaction();

        //        var sektor = new Sektor
        //        {
        //            Naziv = p.Naziv,
        //            TipSektora = p.TipSektora,
        //            Kapacitet = p.Kapacitet,
        //            Povrsina = p.Povrsina,
        //        };

        //        // Postavljanje reference na vođu sektora
        //        if (p.VodjaSektora != null)
        //        {
        //            var vodja = s.Load<Stanovnik>(p.VodjaSektora.Id);
        //            sektor.VodjaSektora = vodja;
        //        }

        //        s.Save(sektor);
        //        t.Commit();
        //        return sektor;
        //    }
        //    catch (Exception ex)
        //    {
        //        t?.Rollback();
        //        Console.Error.WriteLine(ex.Message);
        //        throw;
        //    }
        //    finally
        //    {
        //        s?.Close();
        //    }
        //}

        public static SektorDetalji DodajSektor(SektorDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                var sektor = new Sektor
                {
                    Naziv = p.Naziv,
                    TipSektora = p.TipSektora,
                    Kapacitet = p.Kapacitet,
                    Povrsina = p.Povrsina,
                };

                if (p.VodjaSektora != null)
                {
                    var vodja = s.Get<Stanovnik>(p.VodjaSektora.Id);
                    sektor.VodjaSektora = vodja;
                }

                s.Save(sektor);

                // ===== KLJUČNA IZMENA OVDE =====
                if (p.Radnici != null)
                {
                    foreach (var radnikDTO in p.Radnici)
                    {
                        var radnik = s.Get<Stanovnik>(radnikDTO.Id);
                        if (radnik != null)
                        {
                            // Jednostavno dodamo učitanog radnika u kolekciju sektora.
                            // NHibernate će sam kreirati red u prelaznoj tabeli 'RADI_U'.
                            sektor.Radnici.Add(radnik);
                        }
                    }
                }
                // ===== KRAJ IZMENE =====

                // Ne treba nam s.Update(sektor) jer NHibernate prati promene,
                // ali commit će se pobrinuti za sve.
                t.Commit();

                // Pretvaramo sačuvani ENTITET nazad u DTO DOK JE SESIJA JOŠ OTVORENA
                return new SektorDetalji
                {
                    Id = sektor.Id,
                    Naziv = sektor.Naziv,
                    TipSektora = sektor.TipSektora,
                    Kapacitet = sektor.Kapacitet,
                    Povrsina = sektor.Povrsina,
                    VodjaSektora = sektor.VodjaSektora != null ? new StanovnikPregled
                    {
                        Id = sektor.VodjaSektora.Id,
                        Ime = sektor.VodjaSektora.Ime,
                        Prezime = sektor.VodjaSektora.Prezime,
                        Zanimanje = sektor.VodjaSektora.Zanimanje
                    } : null,
                    Radnici = sektor.Radnici.Select(radnik => new StanovnikPregled
                    {
                        Id = radnik.Id,
                        Ime = radnik.Ime,
                        Prezime = radnik.Prezime,
                        Zanimanje = radnik.Zanimanje
                    }).ToList(),
                    ResursiUSektoru = new List<ResursPregled>(),
                    IstorijaOdrzavanja = new List<ZapisOdrzavanjaPregled>()
                };
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Ažurira postojeći sektor u bazi unutar transakcije.
        /// </summary>
        public static void AzurirajSektor(SektorDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var sektor = s.Load<Sektor>(p.Id);

                sektor.Naziv = p.Naziv;
                sektor.TipSektora = p.TipSektora;
                sektor.Kapacitet = p.Kapacitet;
                sektor.Povrsina = p.Povrsina;

                // Ažuriranje reference na vođu sektora
                if (p.VodjaSektora != null)
                {
                    var vodja = s.Load<Stanovnik>(p.VodjaSektora.Id);
                    sektor.VodjaSektora = vodja;
                }
                else
                {
                    sektor.VodjaSektora = null;
                }

                s.Update(sektor);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();

                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše sektor iz baze unutar transakcije.
        /// </summary>
        /// 
        //MENJANO iz bool u void al sad je fora sto su contorlleri prejebani kad se ovo promenilo
        public static void ObrisiSektor(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            // 2. IZMENA: 'validnoObrisan' promenljiva je nepotrebna i obrisana je.
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                var sektor = s.Load<Sektor>(id);
                s.Delete(sektor);

                t.Commit();
                // 3. IZMENA: Nema više 'return true' na kraju try bloka.
                // Ako stigne do ovde, metoda se uspešno završava.
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);

                // Jedini zadatak catch bloka je da prosledi grešku dalje.
                // return naredba je obrisana.
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        //MENJANO iz bool u void takodje sad je prblem sto controller jede govna
        public static void DodeliRadnikaSektoru(int radnikId, int sektorId)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                // Provera da li veza već postoji
                bool vezaPostoji = s.Query<RadiU>().Any(r => r.Radnik.Id == radnikId && r.SektorGdeRadi.Id == sektorId);

                // 2. IZMENA: Ako veza već postoji, posao je završen. Izađi iz metode.
                if (vezaPostoji)
                {
                    // Nema potrebe za commitom prazne transakcije, možemo odmah izaći.
                    // Finally blok će se svakako izvršiti.
                    return;
                }

                // Ako veza ne postoji, nastavljamo sa kreiranjem
                var radnik = s.Load<Stanovnik>(radnikId);
                var sektor = s.Load<Sektor>(sektorId);

                var novaVeza = new RadiU
                {
                    Radnik = radnik,
                    SektorGdeRadi = sektor
                };

                s.Save(novaVeza);
                t.Commit();
                // Nema 'return true', ako je stigao dovde, uspeo je.
            }
            catch (Exception ex)
            {
                // 3. IZMENA: Ispravljen catch blok
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw; // Samo prosleđujemo grešku dalje
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Uklanja radnika iz sektora (raskida M:N vezu).
        /// </summary>
        public static void UkloniRadnikaIzSektora(int radnikId, int sektorId)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var vezaZaBrisanje = s.Query<RadiU>()
                                     .FirstOrDefault(r => r.Radnik.Id == radnikId && r.SektorGdeRadi.Id == sektorId);

                if (vezaZaBrisanje != null)
                {
                    s.Delete(vezaZaBrisanje);
                }

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        #endregion

        #region Zadatak

        /// <summary>
        /// Pomoćna privatna funkcija koja konvertuje Zadatak entitet u odgovarajući konkretni ZadatakPregled DTO.
        /// </summary>
        private static ZadatakPregled? KreirajZadatakPregled(Zadatak zadatak)
        {
            if (zadatak == null)
                return null;

            ZadatakPregled pregled;

            if (zadatak is Istrazivanje)
                pregled = new IstrazivanjePregled { TipZadatka = "Istrazivanje" };
            else if (zadatak is Odrzavanje)
                pregled = new OdrzavanjePregled { TipZadatka = "Odrzavanje" };
            else if (zadatak is Evakuacija)
                pregled = new EvakuacijaPregled { TipZadatka = "Evakuacija" };
            else if (zadatak is Eksperiment)
                pregled = new EksperimentPregled { TipZadatka = "Eksperiment" };
            else if (zadatak is MedicinskaIntervencija)
                pregled = new MedicinskaIntervencijaPregled { TipZadatka = "Medicinska Intervencija" };
            else
                return null; // Nepoznat tip zadatka

            pregled.Id = zadatak.Id;
            pregled.OpisCilja = zadatak.OpisCilja;
            pregled.DatumPocetka = zadatak.DatumPocetka;

            return pregled;
        }

        public static List<ZadatakPregled> VratiSveZadatke()
        {
            var zadaciPregled = new List<ZadatakPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sviZadaci = s.Query<Zadatak>().ToList();

                foreach (var zadatak in sviZadaci)
                {
                    // Koristimo pomoćnu funkciju da dobijemo ispravan DTO
                    var pregled = KreirajZadatakPregled(zadatak);
                    if (pregled != null)
                    {
                        zadaciPregled.Add(pregled);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return zadaciPregled;
        }
        //MENJANO JAKO 
        public static ZadatakDetalji? VratiZadatakDetalji(int id)
        {
            // Lokalne promenljive su ispravno nullable
            ZadatakDetalji? detalji = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                // Koristimo Get() koji vraća null ako entitet ne postoji
                var zadatak = s.Get<Zadatak>(id);

                // 2. IZMENA: KRITIČNA PROVERA koja sprečava pad programa!
                // Ako zadatak nije nađen, odmah vraćamo null.
                if (zadatak == null)
                {
                    return null;
                }

                // Odavde pa na dalje, kompajler zna da 'zadatak' NIJE null.
                switch (zadatak)
                {
                    case Evakuacija z_evak:
                        detalji = new EvakuacijaDetalji
                        {
                            OblastEvakuacije = z_evak.OblastEvakuacije,
                            BrojOsobaEvakuacije = z_evak.BrojOsobaEvakuacije
                        };
                        break;
                    // ... ostali case-ovi ...
                    case Eksperiment z_exp:
                        detalji = new EksperimentDetalji { NivoOpasnosti = z_exp.NivoOpasnosti };
                        break;
                    case MedicinskaIntervencija z_med:
                        detalji = new MedicinskaIntervencijaDetalji
                        {
                            BrojPovredjenih = z_med.BrojPovredjenih,
                            StepenKriticnosti = z_med.StepenKriticnosti
                        };
                        break;
                    case Odrzavanje _:
                        detalji = new OdrzavanjeDetalji();
                        break;
                    case Istrazivanje _:
                        detalji = new IstrazivanjeDetalji();
                        break;
                    default:
                        throw new InvalidOperationException($"Nije implementirano kreiranje DTO objekta za tip zadatka: {zadatak.GetType().Name}");
                }

                // Odavde pa na dalje, kompajler zna i da 'detalji' NIJE null, jer bi 'default' grana bacila grešku.
                detalji.Id = zadatak.Id;
                detalji.OpisCilja = zadatak.OpisCilja;
                detalji.Lokacija = zadatak.Lokacija;
                detalji.OcekivanoTrajanjeDani = zadatak.OcekivanoTrajanjeDani;
                detalji.DatumPocetka = zadatak.DatumPocetka;
                detalji.DatumZavrsetka = zadatak.DatumZavrsetka;
                detalji.OcenaUspesnosti = zadatak.OcenaUspesnosti;
                detalji.Rezultat = zadatak.Rezultat;
                detalji.BrojPotrebnihUcesnika = zadatak.BrojPotrebnihUcesnika;

                detalji.Podzadatak = (zadatak.Nadzadatak != null) ? KreirajZadatakPregled(zadatak.Nadzadatak) : null;

                // 3. IZMENA: Dodat .Where() da se izbace potencijalni null rezultati
                detalji.Podzadaci = zadatak.Podzadaci
                                          .Select(p => KreirajZadatakPregled(p))
                                          .Where(p => p != null) // Izbacuje sve null vrednosti
                                          .ToList()!;           // Znak '!' govori kompajleru da smo sigurni da lista nije null

                detalji.PotrebneSpecijalizacije = zadatak.PotrebneSpecijalizacije.Select(p => new PotrebnaPregled { Id = p.Id, NazivSpecijalizacije = p.Specijalizacija.Naziv, Nivo = p.Nivo }).ToList();

                foreach (var angazman in zadatak.AngazovaniUcesnici)
                {
                    var ucesnikWrapper = angazman.Ucesnik;
                    if (ucesnikWrapper != null)
                    {
                        var pregled = new AngazovaniUcesnikPregled();
                        if (ucesnikWrapper.PripadaStanovniku != null)
                        {
                            pregled.Tip = "Stanovnik";
                            pregled.Naziv = ucesnikWrapper.PripadaStanovniku.Ime;
                            detalji.AngazovaniUcesnici.Add(pregled);
                        }
                        else if (ucesnikWrapper.PripadaRobotu != null)
                        {
                            pregled.Tip = "Robot";
                            pregled.Naziv = ucesnikWrapper.PripadaRobotu.Sifra;
                            detalji.AngazovaniUcesnici.Add(pregled);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // 4. IZMENA: Ispravljen catch blok
                throw; // U slučaju bilo kakve neočekivane greške, prosledi je dalje
            }
            finally
            {
                s?.Close();
            }
            return detalji;
        }

        // Metode DodajZadatak, AzurirajZadatak i ObrisiZadatak ostaju iste kao u prethodnom odgovoru
        // ... (da ih ne bih ponavljao bez potrebe) ...
        public static void DodajZadatak(ZadatakDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                Zadatak? noviZadatak = null;

                if (p is EvakuacijaDetalji evak)
                    noviZadatak = new Evakuacija { OblastEvakuacije = evak.OblastEvakuacije, BrojOsobaEvakuacije = evak.BrojOsobaEvakuacije };
                else if (p is EksperimentDetalji exp)
                    noviZadatak = new Eksperiment { NivoOpasnosti = exp.NivoOpasnosti };
                else if (p is MedicinskaIntervencijaDetalji med)
                    noviZadatak = new MedicinskaIntervencija { BrojPovredjenih = med.BrojPovredjenih, StepenKriticnosti = med.StepenKriticnosti };
                else if (p is OdrzavanjeDetalji)
                    noviZadatak = new Odrzavanje();
                else if (p is IstrazivanjeDetalji)
                    noviZadatak = new Istrazivanje();

                if (noviZadatak != null)
                {
                    noviZadatak.OpisCilja = p.OpisCilja;
                    noviZadatak.Lokacija = p.Lokacija;
                    noviZadatak.OcekivanoTrajanjeDani = p.OcekivanoTrajanjeDani;
                    noviZadatak.DatumPocetka = p.DatumPocetka;
                    noviZadatak.DatumZavrsetka = p.DatumZavrsetka;

                    s.Save(noviZadatak);
                    t.Commit();
                }
                else
                {
                    throw new InvalidOperationException("Tip zadatka nije prepoznat i ne može biti sačuvan.");
                }
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        public static void AzurirajZadatak(int id, ZadatakDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                // Učitavamo zadatak iz baze po ID-ju koji je stigao iz rute.
                Zadatak zadatak = s.Get<Zadatak>(id);

                if (zadatak == null)
                {
                    throw new InvalidOperationException($"Zadatak sa ID-jem {id} ne postoji.");
                }

                string stvarniTipZadatka = zadatak.GetType().Name;

                // Mapiranje DTO tipa u stvarni tip entiteta
                string ocekivaniTipEntiteta = "";
                switch (p)
                {
                    case EksperimentDetalji: ocekivaniTipEntiteta = "Eksperiment"; break;
                    case EvakuacijaDetalji: ocekivaniTipEntiteta = "Evakuacija"; break;
                    case MedicinskaIntervencijaDetalji: ocekivaniTipEntiteta = "MedicinskaIntervencija"; break;
                    case OdrzavanjeDetalji: ocekivaniTipEntiteta = "Odrzavanje"; break;
                    case IstrazivanjeDetalji: ocekivaniTipEntiteta = "Istrazivanje"; break;
                }

                if (stvarniTipZadatka != ocekivaniTipEntiteta)
                {
                    // Sada koristimo ToString() za lepšu poruku
                    throw new InvalidOperationException(
                        $"Pokušavate da izmenite zadatak tipa '{stvarniTipZadatka}' ali ste koristili metodu za '{p.ToString()}'."
                    );
                }
                // ===== KRAJ PROVERE =====

                // Ako je provera prošla, nastavljamo sa ažuriranjem podataka.
                zadatak.OpisCilja = p.OpisCilja;
                zadatak.Lokacija = p.Lokacija;
                zadatak.OcekivanoTrajanjeDani = p.OcekivanoTrajanjeDani;
                zadatak.DatumPocetka = p.DatumPocetka;
                zadatak.DatumZavrsetka = p.DatumZavrsetka;
                zadatak.Rezultat = p.Rezultat;
                zadatak.OcenaUspesnosti = p.OcenaUspesnosti;
                zadatak.BrojPotrebnihUcesnika = p.BrojPotrebnihUcesnika;

                // Ažuriranje specifičnih svojstava (sada je sigurno jer smo proverili tip)
                if (zadatak is Evakuacija z_evak && p is EvakuacijaDetalji evak)
                {
                    z_evak.OblastEvakuacije = evak.OblastEvakuacije;
                    z_evak.BrojOsobaEvakuacije = evak.BrojOsobaEvakuacije;
                }
                else if (zadatak is Eksperiment z_exp && p is EksperimentDetalji exp)
                {
                    z_exp.NivoOpasnosti = exp.NivoOpasnosti;
                }
                else if (zadatak is MedicinskaIntervencija z_med && p is MedicinskaIntervencijaDetalji med)
                {
                    z_med.BrojPovredjenih = med.BrojPovredjenih;
                    z_med.StepenKriticnosti = med.StepenKriticnosti;
                }

                s.Update(zadatak);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw; // Uvek prosleđujemo grešku dalje ka kontroleru
            }
            finally
            {
                s?.Close();
            }
        }

        public static void ObrisiZadatak(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var zadatak = s.Load<Zadatak>(id);
                s.Delete(zadatak);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        public static void AngazujUcesnikaNaZadatku(int ucesnikId, string tipUcesnika, int zadatakId)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var zadatak = s.Get<Zadatak>(zadatakId);

                UcesnikZadatka? ucesnikZadatka = null;
                if (tipUcesnika == "Stanovnik")
                {
                    ucesnikZadatka = s.Query<UcesnikZadatka>().FirstOrDefault(u => u.PripadaStanovniku != null && u.PripadaStanovniku.Id == ucesnikId);
                }
                else if (tipUcesnika == "Robot")
                {
                    ucesnikZadatka = s.Query<UcesnikZadatka>().FirstOrDefault(u => u.PripadaRobotu != null && u.PripadaRobotu.Id == ucesnikId);
                }

                if (zadatak == null || ucesnikZadatka == null)
                {
                    Console.WriteLine("Greška: Zadatak ili Učesnik nisu pronađeni.");
                    return;
                }

                bool vecPostoji = s.Query<AngazovanNa>().Any(a => a.Zadatak.Id == zadatakId && a.Ucesnik.Id == ucesnikZadatka.Id);
                if (vecPostoji)
                {
                    Console.WriteLine("Ovaj učesnik je već angažovan na zadatku.");
                    return;
                }

                var noviAngazman = new AngazovanNa
                {
                    Zadatak = zadatak,
                    Ucesnik = ucesnikZadatka
                };

                s.Save(noviAngazman);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        #endregion

        #region Robot

        /// <summary>
        /// Vraća listu svih robota sa osnovnim podacima.
        /// </summary>
        public static List<RobotPregled> VratiSveRobote()
        {
            var roboti = new List<RobotPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sviRoboti = s.Query<Robot>().ToList();

                foreach (var r in sviRoboti)
                {
                    roboti.Add(new RobotPregled
                    {
                        Id = r.Id,
                        Sifra = r.Sifra,
                        Tip = r.Tip
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return roboti;
        }

        /// <summary>
        /// Vraća sve detaljne informacije o jednom robotu.
        /// </summary>
        public static RobotDetalji? VratiRobotaDetalji(int id)
        {
            RobotDetalji? robot = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                // 2. IZMENA: Ključna promena sa Load() na Get()
                var r = s.Get<Robot>(id);

                // 3. IZMENA: Proveravamo da li je Get() vratio null
                if (r == null)
                {
                    // Robot nije pronađen, vraćamo null. Kontroler će ovo pretvoriti u 404 Not Found.
                    return null;
                }

                // Ovaj deo koda se izvršava samo ako je robot 'r' pronađen
                robot = new RobotDetalji
                {
                    Id = r.Id,
                    Tip = r.Tip,
                    Sifra = r.Sifra,
                    OdgovorniStanovnik = r.OdgovorniStanovnik != null ? new StanovnikPregled
                    {
                        Id = r.OdgovorniStanovnik.Id,
                        Ime = r.OdgovorniStanovnik.Ime,
                        Prezime = r.OdgovorniStanovnik.Prezime,
                        Zanimanje = r.OdgovorniStanovnik.Zanimanje
                    } : null,
                    Programer = r.Programer != null ? new StanovnikPregled
                    {
                        Id = r.Programer.Id,
                        Ime = r.Programer.Ime,
                        Prezime = r.Programer.Prezime,
                        Zanimanje = r.Programer.Zanimanje
                    } : null,
                    Sektor = r.Sektor != null ? new SektorPregled
                    {
                        Id = r.Sektor.Id,
                        Naziv = r.Sektor.Naziv,
                        TipSektora = r.Sektor.TipSektora
                    } : null
                };
            }
            catch (Exception ex)
            {
                // Catch blok je za neočekivane greške servera
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }

            return robot;
        }

        /// <summary>
        /// Dodaje novog robota u bazu unutar transakcije.
        /// </summary>
        public static void DodajRobota(RobotDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                // 1. KREIRAMO NOVI 'UcesnikZadatka' OBJEKAT
                // On je "prazan", služi samo kao veza
                var noviUcesnik = new UcesnikZadatka();

                // 2. KREIRAMO ROBOTA
                var robot = new Robot
                {
                    Sifra = p.Sifra,
                    Tip = p.Tip
                    // 'UcesnikZadatka' svojstvo se ne postavlja direktno ovde
                };

                // 3. USPOSTAVLJAMO DVOSTRUKU VEZU
                noviUcesnik.PripadaRobotu = robot; // Ucesnik pokazuje na Robota
                robot.UcesnikZadatka = noviUcesnik;  // Robot pokazuje na Ucesnika

                // Ostatak koda ostaje isti...
                if (p.OdgovorniStanovnik != null)
                {
                    var odgovorni = s.Load<Stanovnik>(p.OdgovorniStanovnik.Id);
                    robot.OdgovorniStanovnik = odgovorni;
                }
                if (p.Programer != null)
                {
                    var programer = s.Load<Stanovnik>(p.Programer.Id);
                    robot.Programer = programer;
                }
                if (p.Sektor != null)
                {
                    var sektor = s.Load<Sektor>(p.Sektor.Id);
                    robot.Sektor = sektor;
                }

                // 4. ČUVAMO OBJEKAT ZA KOJI BAZA NEĆE GENERISATI ID
                // NHibernate će sada znati da prvo mora da sačuva 'noviUcesnik'
                // da bi dobio njegov ID, a zatim će taj ID upotrebiti pri čuvanju 'robota'.
                // U zavisnosti od 'cascade' podešavanja, možda je dovoljno sačuvati samo 'robota'.
                // Hajde da probamo sigurniju varijantu - čuvamo obe strane.
                s.Save(noviUcesnik);
                s.Save(robot);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.ToString()); // Koristite ToString() za više detalja
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Ažurira postojećeg robota u bazi unutar transakcije.
        /// </summary>
        public static void AzurirajRobota(RobotDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var robot = s.Load<Robot>(p.Id);

                robot.Sifra = p.Sifra;
                robot.Tip = p.Tip;

                if (p.OdgovorniStanovnik != null)
                {
                    var odgovorni = s.Load<Stanovnik>(p.OdgovorniStanovnik.Id);
                    robot.OdgovorniStanovnik = odgovorni;
                }
                else { robot.OdgovorniStanovnik = null; }

                if (p.Programer != null)
                {
                    var programer = s.Load<Stanovnik>(p.Programer.Id);
                    robot.Programer = programer;
                }
                else { robot.Programer = null; }

                if (p.Sektor != null)
                {
                    var sektor = s.Load<Sektor>(p.Sektor.Id);
                    robot.Sektor = sektor;
                }
                else { robot.Sektor = null; }

                s.Update(robot);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše robota iz baze unutar transakcije.
        /// </summary>
        public static void ObrisiRobota(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var robot = s.Load<Robot>(id);
                s.Delete(robot);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        #endregion

        #region Resurs

        /// <summary>
        /// Vraća listu svih resursa sa osnovnim podacima.
        /// </summary>
        public static List<ResursPregled> VratiSveResurse()
        {
            var resursi = new List<ResursPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sviResursi = s.Query<Resurs>().ToList();
                foreach (var r in sviResursi)
                {
                    resursi.Add(new ResursPregled
                    {
                        Id = r.Id,
                        Naziv = r.Naziv,
                        TrenutnaKolicina = r.TrenutnaKolicina
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return resursi;
        }

        /// <summary>
        /// Vraća sve detaljne informacije o jednom resursu.
        /// </summary>
        public static ResursDetalji? VratiResursDetalji(int id)
        {
            ResursDetalji? detalji = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                // 2. IZMENA: Ključna promena sa Load() na Get()
                var r = s.Get<Resurs>(id);

                // 3. IZMENA: Proveravamo rezultat ODMAH
                // Ako Get() nije našao resurs, r će biti null.
                if (r == null)
                {
                    // Vraćamo null kontroleru, koji će ovo pretvoriti u NotFound (404)
                    return null;
                }

                // Ovaj kod se izvršava samo ako je resurs 'r' uspešno pronađen
                detalji = new ResursDetalji
                {
                    Id = r.Id,
                    Naziv = r.Naziv,
                    TrenutnaKolicina = r.TrenutnaKolicina,
                    Skladiste = r.Sektor != null ? new SektorPregled
                    {
                        Id = r.Sektor.Id,
                        Naziv = r.Sektor.Naziv,
                        TipSektora = r.Sektor.TipSektora
                    } : null,
                    Upravitelji = r.UpravljaVeze.Select(veza => new StanovnikPregled
                    {
                        Id = veza.Stanovnik.Id,
                        Ime = veza.Stanovnik.Ime,
                        Prezime = veza.Stanovnik.Prezime,
                        Zanimanje = veza.Stanovnik.Zanimanje
                    }).ToList(),
                    PotrosnjaPoSektorima = r.PotrosnjaPoSektorima.Select(p => new TrosiPregled
                    {
                        Id = p.Id,
                        NazivSektora = p.Sektor.Naziv,
                        NazivResursa = p.Resurs.Naziv,
                        DnevniProsek = p.DnevniProsek,
                        KriticnaVrednost = p.KriticnaVrednost
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                // Catch blok sada hvata samo PRAVE greške servera (npr. pad baze)
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return detalji;
        }

        /// <summary>
        /// Dodaje novi resurs u bazu unutar transakcije.
        /// </summary>

        //public static ResursDetalji DodajResurs(ResursDetalji p)
        //{
        //    ISession? s = null;
        //    ITransaction? t = null;
        //    try
        //    {
        //        s = DataLayer.GetSession();
        //        if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

        //        t = s.BeginTransaction();

        //        // Kreiramo entitet 'Resurs'
        //        var resurs = new Resurs
        //        {
        //            Naziv = p.Naziv,
        //            TrenutnaKolicina = p.TrenutnaKolicina
        //        };

        //        if (p.Skladiste != null)
        //        {
        //            var skladiste = s.Get<Sektor>(p.Skladiste.Id);
        //            resurs.Sektor = skladiste;
        //        }

        //        s.Save(resurs);

        //        if (p.Upravitelji != null)
        //        {
        //            foreach (var upraviteljDTO in p.Upravitelji)
        //            {
        //                var stanovnik = s.Get<Stanovnik>(upraviteljDTO.Id);
        //                if (stanovnik != null)
        //                {
        //                    var novaVeza = new UpravljaResursom
        //                    {
        //                        Resurs = resurs,
        //                        Stanovnik = stanovnik
        //                    };
        //                    resurs.UpravljaVeze.Add(novaVeza);
        //                    s.Save(novaVeza);
        //                }
        //            }
        //        }

        //        t.Commit();

        //        // 2. KLJUČNA IZMENA: Pretvaramo sačuvani ENTITET nazad u DTO DOK JE SESIJA JOŠ OTVORENA
        //        // Sada će NHibernate uspešno učitati sve potrebne podatke (imena stanovnika itd.)
        //        return new ResursDetalji
        //        {
        //            Id = resurs.Id,
        //            Naziv = resurs.Naziv,
        //            TrenutnaKolicina = resurs.TrenutnaKolicina,
        //            Skladiste = resurs.Sektor != null ? new SektorPregled { Id = resurs.Sektor.Id, Naziv = resurs.Sektor.Naziv, TipSektora = resurs.Sektor.TipSektora } : null,
        //            Upravitelji = resurs.UpravljaVeze.Select(veza => new StanovnikPregled
        //            {
        //                Id = veza.Stanovnik.Id,
        //                Ime = veza.Stanovnik.Ime,
        //                Prezime = veza.Stanovnik.Prezime,
        //                Zanimanje = veza.Stanovnik.Zanimanje
        //            }).ToList(),
        //            PotrosnjaPoSektorima = new List<TrosiPregled>() // Vraćamo praznu listu jer je nismo ni menjali
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        t?.Rollback();
        //        Console.Error.WriteLine(ex.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //        s?.Close();
        //    }
        //}

        public static ResursDetalji DodajResurs(ResursDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                if (s == null) throw new InvalidOperationException("Sesija ka bazi nije uspostavljena.");

                t = s.BeginTransaction();

                var resurs = new Resurs
                {
                    Naziv = p.Naziv,
                    TrenutnaKolicina = p.TrenutnaKolicina
                };

                if (p.Skladiste != null)
                {
                    var skladiste = s.Get<Sektor>(p.Skladiste.Id);
                    resurs.Sektor = skladiste;
                }

                s.Save(resurs);

                if (p.Upravitelji != null)
                {
                    foreach (var upraviteljDTO in p.Upravitelji)
                    {
                        var stanovnik = s.Get<Stanovnik>(upraviteljDTO.Id);
                        if (stanovnik != null)
                        {
                            var novaVeza = new UpravljaResursom { Resurs = resurs, Stanovnik = stanovnik };
                            resurs.UpravljaVeze.Add(novaVeza);
                            s.Save(novaVeza);
                        }
                    }
                }

                // ===== NOVI BLOK KODA ZA OBRADU POTROŠNJE =====
                if (p.PotrosnjaPoSektorima != null)
                {
                    foreach (var potrosnjaDTO in p.PotrosnjaPoSektorima)
                    {
                        // Učitavamo sektor za koji se vezuje potrošnja
                        var sektorPotrosnje = s.Get<Sektor>(potrosnjaDTO.SektorId);
                        if (sektorPotrosnje != null)
                        {
                            // Kreiramo novi 'Trosi' entitet
                            var novaPotrosnja = new Trosi
                            {
                                Resurs = resurs,
                                Sektor = sektorPotrosnje,
                                DnevniProsek = potrosnjaDTO.DnevniProsek,
                                KriticnaVrednost = potrosnjaDTO.KriticnaVrednost
                            };
                            // Dodajemo ga u memorijsku kolekciju resursa
                            resurs.PotrosnjaPoSektorima.Add(novaPotrosnja);
                            // Čuvamo ga u bazi
                            s.Save(novaPotrosnja);
                        }
                    }
                }
                // ===== KRAJ NOVOG BLOKA =====

                t.Commit();

                // Sada kada kreiramo odgovor, PotrosnjaPoSektorima više neće biti prazna
                return new ResursDetalji
                {
                    Id = resurs.Id,
                    Naziv = resurs.Naziv,
                    TrenutnaKolicina = resurs.TrenutnaKolicina,
                    Skladiste = resurs.Sektor != null ? new SektorPregled { Id = resurs.Sektor.Id, Naziv = resurs.Sektor.Naziv, TipSektora = resurs.Sektor.TipSektora } : null,
                    Upravitelji = resurs.UpravljaVeze.Select(veza => new StanovnikPregled
                    {
                        Id = veza.Stanovnik.Id,
                        Ime = veza.Stanovnik.Ime,
                        Prezime = veza.Stanovnik.Prezime,
                        Zanimanje = veza.Stanovnik.Zanimanje
                    }).ToList(),
                    // ISPRAVLJENO: Sada mapiramo podatke iz sačuvanog entiteta
                    PotrosnjaPoSektorima = resurs.PotrosnjaPoSektorima.Select(potrosnja => new TrosiPregled
                    {
                        Id = potrosnja.Id,
                        SektorId = potrosnja.Sektor.Id,
                        NazivSektora = potrosnja.Sektor.Naziv,
                        NazivResursa = potrosnja.Resurs.Naziv,
                        DnevniProsek = potrosnja.DnevniProsek,
                        KriticnaVrednost = potrosnja.KriticnaVrednost
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Ažurira postojeći resurs u bazi unutar transakcije.
        /// </summary>
        public static void AzurirajResurs(ResursDetalji p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var resurs = s.Load<Resurs>(p.Id);

                resurs.Naziv = p.Naziv;
                resurs.TrenutnaKolicina = p.TrenutnaKolicina;

                if (p.Skladiste != null)
                {
                    var skladiste = s.Load<Sektor>(p.Skladiste.Id);
                    resurs.Sektor = skladiste;
                }
                else
                {
                    resurs.Sektor = null;
                }

                // 3. BRIŠEMO sve postojeće veze između ovog resursa i stanovnika
                // Ovo je najjednostavniji i najsigurniji način za sinhronizaciju
                var postojeceVeze = s.Query<UpravljaResursom>()
                                     .Where(v => v.Resurs.Id == resurs.Id)
                                     .ToList();

                foreach (var veza in postojeceVeze)
                {
                    s.Delete(veza);
                }

                // *** KLJUČNA IZMENA ***
                // Nateraj NHibernate da odmah izvrši sve DELETE komande iznad.
                // Ovo čisti tabelu pre nego što pokušamo da dodamo nove (potencijalno iste) veze.
                s.Flush();


                // 4. DODAJEMO PONOVO sve veze na osnovu liste koja je stigla sa forme
                if (p.Upravitelji != null)
                { 
                    foreach (var upraviteljDTO in p.Upravitelji)
                    {
                        var stanovnik = s.Load<Stanovnik>(upraviteljDTO.Id);

                        var novaVeza = new UpravljaResursom
                        {
                            Resurs = resurs,
                            Stanovnik = stanovnik
                        };

                        s.Save(novaVeza);
                    }
                }

                // Nije neophodno zvati s.Update(resurs) jer NHibernate prati promene
                // na učitanom entitetu, ali ne smeta ako stoji.
                //s.Update(resurs);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše resurs iz baze unutar transakcije.
        /// </summary>
        public static void ObrisiResurs(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var resurs = s.Load<Resurs>(id);
                s.Delete(resurs);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        public static bool DodeliUpraviteljaResursu(int stanovnikId, int resursId)
        {
            ISession? s = null;
            ITransaction? t = null;
            bool validno = true;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                // 1. Učitaj entitete
                var stanovnik = s.Load<Stanovnik>(stanovnikId);
                var resurs = s.Load<Resurs>(resursId);

                // 2. Proveri da li veza već postoji (opciono ali dobra praksa)
                bool vezaPostoji = s.Query<UpravljaResursom>()
                                    .Any(v => v.Resurs.Id == resursId && v.Stanovnik.Id == stanovnikId);

                if (!vezaPostoji)
                {
                    // 3. Kreiraj NOVI objekat prelazne klase
                    var novaVeza = new UpravljaResursom
                    {
                        Resurs = resurs,
                        Stanovnik = stanovnik
                    };

                    // 4. Sačuvaj taj novi objekat
                    s.Save(novaVeza);
                }

                t.Commit();
            }
            catch (Exception ex)
            {
                validno = false;
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return validno;
        }

        #endregion

        #region Specijalizacija

        /// <summary>
        /// Vraća listu svih dostupnih specijalizacija.
        /// </summary>
        public static List<SpecijalizacijaPregled> VratiSveSpecijalizacije()
        {
            var specijalizacije = new List<SpecijalizacijaPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sveSpecijalizacije = s.Query<Specijalizacija>().ToList();
                foreach (var spec in sveSpecijalizacije)
                {
                    specijalizacije.Add(new SpecijalizacijaPregled
                    {
                        Id = spec.Id,
                        Naziv = spec.Naziv
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return specijalizacije;
        }

        public static void DodajSpecijalizaciju(SpecijalizacijaPregled p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var specijalizacija = new Specijalizacija
                {
                    Naziv = p.Naziv
                };
                var noviId = s.CreateSQLQuery("SELECT SEQ_SPECIJALIZACIJA.NEXTVAL FROM DUAL").UniqueResult<decimal>();
                specijalizacija.Id = Convert.ToInt32(noviId);

                s.Save(specijalizacija);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        public static void AzurirajSpecijalizaciju(SpecijalizacijaPregled p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var specijalizacija = s.Load<Specijalizacija>(p.Id);
                specijalizacija.Naziv = p.Naziv;

                s.Update(specijalizacija);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        public static void ObrisiSpecijalizaciju(int id)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var specijalizacija = s.Load<Specijalizacija>(id);
                s.Delete(specijalizacija);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        #endregion

        #region ZapisOdrzavanja

        /// <summary>
        /// Vraća listu svih zapisa o održavanjima.
        /// </summary>
        public static List<ZapisOdrzavanjaPregled> VratiSveZapiseOdrzavanja()
        {
            var zapisi = new List<ZapisOdrzavanjaPregled>();
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var sviZapisi = s.Query<ZapisOdrzavanja>().ToList();
                foreach (var z in sviZapisi)
                {
                    zapisi.Add(new ZapisOdrzavanjaPregled
                    {
                        Id = z.Id,
                        VremeOdrzavanja = z.VremeOdrzavanja,
                        NazivSektora = z.Sektor?.Naziv // Dodajemo ? za slučaj da je sektor null
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return zapisi;
        }

        /// <summary>
        /// Vraća detaljne informacije o jednom zapisu održavanja.
        /// </summary>
        public static ZapisOdrzavanjaDetalji VratiZapisOdrzavanjaDetalji(int id)
        {
            ZapisOdrzavanjaDetalji? detalji = null;
            ISession? s = null;
            try
            {
                s = DataLayer.GetSession();
                var z = s.Load<ZapisOdrzavanja>(id);

                detalji = new ZapisOdrzavanjaDetalji
                {
                    Id = z.Id,
                    VremeOdrzavanja = z.VremeOdrzavanja,
                    Sektor = new SektorPregled
                    {
                        Id = z.Sektor.Id,
                        Naziv = z.Sektor.Naziv,
                        TipSektora = z.Sektor.TipSektora
                    },
                    UcesniciOdrzavanja = z.UcesniciOdrzavanja.Select(u => new StanovnikPregled
                    {
                        Id = u.Id,
                        Ime = u.Ime,
                        Prezime = u.Prezime,
                        Zanimanje = u.Zanimanje
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
            return detalji;
        }


        #endregion

        #region Upravljanje Vezama (Poseduje, Trosi, Potrebna, Kontakt)

        /// <summary>
        /// Dodaje specijalizaciju (posedovanje) određenom stanovniku.
        /// </summary>
        public static void DodajSpecijalizacijuStanovniku(int stanovnikId, int specijalizacijaId, PosedujePregled p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var stanovnik = s.Load<Stanovnik>(stanovnikId);
                var specijalizacija = s.Load<Specijalizacija>(specijalizacijaId);

                var poseduje = new Poseduje
                {
                    Stanovnik = stanovnik,
                    Specijalizacija = specijalizacija,
                    NivoEkspertize = p.NivoEkspertize,
                    DatumSticanja = p.DatumSticanja,
                    Institucija = p.Institucija
                };

                s.Save(poseduje);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše vezu 'Poseduje' (uklanja specijalizaciju stanovniku).
        /// </summary>
        public static void ObrisiPosedujeVezu(int posedujeId)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var poseduje = s.Load<Poseduje>(posedujeId);
                s.Delete(poseduje);

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Dodaje kontakt na Zemlji za određenog stanovnika.
        /// </summary>
        public static void DodajKontaktZaStanovnika(int stanovnikId, KontaktNaZemljiPregled p)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var stanovnik = s.Load<Stanovnik>(stanovnikId);

                var kontakt = new KontaktNaZemlji
                {
                    Stanovnik = stanovnik,
                    Ime = p.Ime,
                    Odnos = p.Odnos
                };
                var noviId = s.CreateSQLQuery("SELECT SEQ_KONTAKT_NA_ZEMLJI.NEXTVAL FROM DUAL").UniqueResult<decimal>();
                kontakt.Id = Convert.ToInt32(noviId);

                foreach (var info in p.KontaktInformacije)
                {
                    var konInfo = new KonZemInf
                    {
                        PripadaKontaktu = kontakt,
                        KontaktInformacija = info
                    };
                    kontakt.KontaktInformacije.Add(konInfo);
                }

                s.Save(kontakt);
                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }

        /// <summary>
        /// Briše kontakt na Zemlji.
        /// </summary>
        public static void ObrisiKontakt(int kontaktId)
        {
            ISession? s = null;
            ITransaction? t = null;
            try
            {
                s = DataLayer.GetSession();
                t = s.BeginTransaction();

                var kontakt = s.Load<KontaktNaZemlji>(kontaktId);
                s.Delete(kontakt); // Cascade će obrisati i KonZemInf

                t.Commit();
            }
            catch (Exception ex)
            {
                t?.Rollback();
                Console.Error.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                s?.Close();
            }
        }


        #endregion
    }
}