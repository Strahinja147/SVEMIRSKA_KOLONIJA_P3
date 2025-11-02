using System;
using System.Collections.Generic;

namespace SVEMIRSKA_KOLONIJA_P3.DTOs
{
    #region Stanovnik

    public class StanovnikPregled
    {
        public int Id { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? Zanimanje { get; set; }
    }

    public class StanovnikDetalji
    {
        public int Id { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public char Pol { get; set; }
        public DateTime? DatumRodjenja { get; set; }
        public string? Nacionalnost { get; set; }
        public string? Zanimanje { get; set; }
        public int? GodineUKoloniji { get; set; }

        public List<KontaktNaZemljiPregled> KontaktiNaZemlji { get; set; } = new List<KontaktNaZemljiPregled>();
        public List<PosedujePregled> Specijalizacije { get; set; } = new List<PosedujePregled>();
        public List<SektorPregled> SektoriKojeVodi { get; set; } = new List<SektorPregled>();
        public List<RobotPregled> OdgovoranZaRobote { get; set; } = new List<RobotPregled>();
        public List<RobotPregled> ProgramiraniRoboti { get; set; } = new List<RobotPregled>();
    }

    #endregion

    #region Sektor

    public class SektorPregled
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }
        public string? TipSektora { get; set; }
    }

    public class SektorDetalji
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }
        public string? TipSektora { get; set; }
        public int? Kapacitet { get; set; }
        public double? Povrsina { get; set; }
        public StanovnikPregled? VodjaSektora { get; set; }

        public List<StanovnikPregled>? Radnici { get; set; } = new List<StanovnikPregled>();
        public List<ResursPregled>? ResursiUSektoru { get; set; } = new List<ResursPregled>();
        public List<ZapisOdrzavanjaPregled>? IstorijaOdrzavanja { get; set; } = new List<ZapisOdrzavanjaPregled>();
    }

    #endregion

    #region Zadatak

    public  class ZadatakPregled// bilo je ovde abstract
    {
        public int Id { get; set; }
        public string? OpisCilja { get; set; }
        public string? TipZadatka { get; set; }
        public DateTime? DatumPocetka { get; set; }

        public ZadatakPregled() { }
        public override string ToString()
        {
            // Prikazuje npr. "12 - Istrazivanje asteroida"
            return $"{this.Id} - {this.OpisCilja}";
        }
    }

    public class IstrazivanjePregled : ZadatakPregled 
    {
        public IstrazivanjePregled() : base() { }
    }
    public class OdrzavanjePregled : ZadatakPregled 
    {
        public OdrzavanjePregled() : base() { }
    }
    public class EvakuacijaPregled : ZadatakPregled 
    {
        public EvakuacijaPregled() : base() { }
    }
    public class EksperimentPregled : ZadatakPregled 
    {
        public EksperimentPregled() : base() { }
    }
    public class MedicinskaIntervencijaPregled : ZadatakPregled 
    {
        public MedicinskaIntervencijaPregled() : base() { }
    }

    public abstract class ZadatakDetalji
    {
        public int Id { get; set; }
        public int? OcekivanoTrajanjeDani { get; set; }
        public DateTime? DatumZavrsetka { get; set; }
        public DateTime? DatumPocetka { get; set; }
        public double? OcenaUspesnosti { get; set; }
        public int? BrojPotrebnihUcesnika { get; set; }
        public string? Rezultat { get; set; }
        public string? OpisCilja { get; set; }
        public string? Lokacija { get; set; }
        public ZadatakPregled? Podzadatak { get; set; }

        public List<ZadatakPregled> Podzadaci { get; set; } = new List<ZadatakPregled>();
        public List<PotrebnaPregled> PotrebneSpecijalizacije { get; set; } = new List<PotrebnaPregled>();
        public List<AngazovaniUcesnikPregled> AngazovaniUcesnici { get; set; } = new List<AngazovaniUcesnikPregled>();

        public override string ToString() => "Zadatak";
    }

    public class AngazovaniUcesnikPregled
    {
        public string? Tip { get; set; }
        public string? Naziv { get; set; }

        public AngazovaniUcesnikPregled() { }
    }

    public class OdrzavanjeDetalji : ZadatakDetalji
    {
        public OdrzavanjeDetalji() : base() { }

        public override string ToString() => "Odrzavanje";
    }
    public class IstrazivanjeDetalji : ZadatakDetalji
    {
        public IstrazivanjeDetalji() : base() { }

        public override string ToString() => "Istrazivanje";
    }
    public class EvakuacijaDetalji : ZadatakDetalji
    {
        public string? OblastEvakuacije { get; set; }
        public int? BrojOsobaEvakuacije { get; set; }
        public EvakuacijaDetalji() : base() { }

        public override string ToString() => "Evakuacija";
    }

    public class EksperimentDetalji : ZadatakDetalji
    {
        public string? NivoOpasnosti { get; set; }
        public EksperimentDetalji() : base() { }

        public override string ToString() => "Eksperiment";
    }

    public class MedicinskaIntervencijaDetalji : ZadatakDetalji
    {
        public int? BrojPovredjenih { get; set; }
        public int? StepenKriticnosti { get; set; }

        public override string ToString() => "MedicinskaIntervencija";
    }

    #endregion

    #region Robot

    public class RobotPregled
    {
        public int Id { get; set; }
        public string? Sifra { get; set; }
        public string? Tip { get; set; }
    }

    public class RobotDetalji
    {
        public int Id { get; set; }
        public string? Tip { get; set; }
        public string? Sifra { get; set; }
        public StanovnikPregled? OdgovorniStanovnik { get; set; }
        public StanovnikPregled? Programer { get; set; }
        public SektorPregled? Sektor { get; set; }
    }

    #endregion

    #region Resurs

    public class ResursPregled
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }
        public int? TrenutnaKolicina { get; set; }
    }

    public class ResursDetalji
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }
        public int? TrenutnaKolicina { get; set; }
        public SektorPregled? Skladiste { get; set; }
        public List<StanovnikPregled>? Upravitelji { get; set; } = new List<StanovnikPregled>();
        public List<TrosiPregled>? PotrosnjaPoSektorima { get; set; } = new List<TrosiPregled>();
    }

    #endregion

    #region Pomoćni DTOs i DTOs za veze

    public class SpecijalizacijaPregled
    {
        public int Id { get; set; }
        public string? Naziv { get; set; }

        public override string ToString()
        {
            return this.Naziv;
        }
    }

    public class KontaktNaZemljiPregled
    {
        public int Id { get; set; }
        public string? Ime { get; set; }
        public string? Odnos { get; set; }
        public List<string> KontaktInformacije { get; set; } = new List<string>();
    }

    public class ZapisOdrzavanjaPregled
    {
        public int Id { get; set; }
        public DateTime? VremeOdrzavanja { get; set; }
        public string? NazivSektora { get; set; }
    }

    public class ZapisOdrzavanjaDetalji
    {
        public int Id { get; set; }
        public DateTime? VremeOdrzavanja { get; set; }
        public SektorPregled? Sektor { get; set; }
        public List<StanovnikPregled> UcesniciOdrzavanja { get; set; } = new List<StanovnikPregled>();
    }

    public class PosedujePregled
    {
        public int Id { get; set; }
        public string? NazivSpecijalizacije { get; set; }
        public string? NivoEkspertize { get; set; }
        public DateTime? DatumSticanja { get; set; }
        public string? Institucija { get; set; }
        public SpecijalizacijaPregled? Specijalizacija { get; set; }
    }

    public class PotrebnaPregled
    {
        public int Id { get; set; }
        public string? NazivSpecijalizacije { get; set; }
        public string? Nivo { get; set; }

        public PotrebnaPregled() { }
    }

    public class TrosiPregled
    {
        public int Id { get; set; }
        public int SektorId { get; set; }
        public string? NazivSektora { get; set; }
        public string? NazivResursa { get; set; }
        public double? DnevniProsek { get; set; }
        public int? KriticnaVrednost { get; set; }
    }

    #endregion
}