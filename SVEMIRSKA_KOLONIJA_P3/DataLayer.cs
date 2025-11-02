using System;
using NHibernate;
using ISession = NHibernate.ISession;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using SVEMIRSKA_KOLONIJA_P3.Mapiranja; // Proverite da li se vaš namespace za mapiranja ovako zove

namespace SVEMIRSKA_KOLONIJA_P3
{
    public class DataLayer
    {
        private static ISessionFactory? _factory = null;
        private static object objLock = new object();

        // Funkcija na zahtev otvara sesiju
        public static ISession GetSession()
        {
            // Ukoliko session factory nije kreiran, kreiraj ga
            if (_factory == null)
            {
                lock (objLock)
                {
                    if (_factory == null)
                        _factory = CreateSessionFactory();
                }
            }

            return _factory.OpenSession();
        }

        // Konfiguracija i kreiranje session factory-ja
        private static ISessionFactory CreateSessionFactory()
        {
            try
            {
                // Konfiguracija za konekciju na Oracle bazu
                // Unesite vaše podatke za konekciju
                var cfg = OracleManagedDataClientConfiguration.Oracle10
                .ConnectionString(c =>
                    c.Is("Data Source=gislab-oracle.elfak.ni.ac.rs:1521/SBP_PDB;User Id=S19089;Password=S19089"));

                return Fluently.Configure()
                    .Database(cfg)
                    // Ovde navedite jednu od vaših klasa za mapiranje
                    // FluentNHibernate će na osnovu nje pronaći sva ostala mapiranja u istom projektu
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<StanovnikMap>()) // Promenite StanovnikMap u bilo koju vašu klasu za mapiranje
                    .BuildSessionFactory();
            }
            catch (Exception ec)
            {
                // Umesto MessageBox-a, koji ne može da se koristi u Web API-ju,
                // bacamo izuzetak koji će biti uhvaćen na višem nivou aplikacije.
                Console.Error.WriteLine(ec.Message);
                throw; // Bacamo originalnu grešku dalje da bi je videli u konzoli ili logovima
            }
        }
    }
}