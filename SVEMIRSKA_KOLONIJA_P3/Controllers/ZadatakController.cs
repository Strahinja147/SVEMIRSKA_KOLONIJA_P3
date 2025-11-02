using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SVEMIRSKA_KOLONIJA_P3; // Obavezno dodati using za namespace gde je DTOManager
using SVEMIRSKA_KOLONIJA_P3.DTOs;
using SVEMIRSKA_KOLONIJA;
using SVEMIRSKA_KOLONIJA_P3.Entiteti;

namespace SVEMIRSKA_KOLONIJA_P3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ZadatakController : ControllerBase
    {
        [HttpGet]
        [Route("VratiSveZadatke")]
        public IActionResult VratiSveZadatke()
        {
            try
            {
                return Ok(DTOManager.VratiSveZadatke());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpGet]
        [Route("VratiZadatak/{id}")]
        public IActionResult VratiZadatak(int id)
        {
            try
            {
                var zadatak = DTOManager.VratiZadatakDetalji(id);
                if (zadatak == null)
                {
                    return NotFound($"Zadatak sa ID-jem {id} nije pronađen.");
                }
                return Ok(zadatak);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpPost]
        [Route("DodajZadatak")]
        public IActionResult DodajZadatak([FromBody] ZadatakDetalji zadatak)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.DodajZadatak(zadatak);
                return StatusCode(201, "Zadatak je uspešno kreiran.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        
        #region Ažuriranje Zadataka

        // JAVNI API ENDPOINTI (po jedan za svaki tip zadatka)

        [HttpPut]
        [Route("AzurirajEksperiment")]
        public IActionResult AzurirajEksperiment([FromBody] EksperimentDetalji eksperiment)
        {
            //if (id != eksperiment.Id)
            //{
            //    return BadRequest("ID u ruti i telu zahteva se ne poklapaju.");
            //}
            // Pozivamo zajedničku, privatnu metodu
            return AzurirajZadatak(eksperiment.Id, eksperiment);
        }

        [HttpPut]
        [Route("AzurirajEvakuaciju")]
        public IActionResult AzurirajEvakuaciju([FromBody] EvakuacijaDetalji evakuacija)
        {
            
            return AzurirajZadatak(evakuacija.Id, evakuacija);
        }

        [HttpPut]
        [Route("AzurirajMedicinskuIntervenciju")]
        public IActionResult AzurirajMedicinskuIntervenciju([FromBody] MedicinskaIntervencijaDetalji intervencija)
        {
            
            return AzurirajZadatak(intervencija.Id, intervencija);
        }

        [HttpPut]
        [Route("AzurirajOdrzavanje")]
        public IActionResult AzurirajOdrzavanje([FromBody] OdrzavanjeDetalji odrzavanje)
        {
            
            return AzurirajZadatak(odrzavanje.Id, odrzavanje);
        }

        [HttpPut]
        [Route("AzurirajIstrazivanje")]
        public IActionResult AzurirajIstrazivanje([FromBody] IstrazivanjeDetalji istrazivanje)
        {
            return AzurirajZadatak(istrazivanje.Id, istrazivanje);
        }


        //PRIVATNA POMOĆNA METODA(sadrži zajedničku logiku)

        //private IActionResult AzurirajZadatak(int id, ZadatakDetalji zadatak)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try
        //    {
        //        // Pozivamo univerzalnu metodu iz DTOManager-a
        //        DTOManager.AzurirajZadatak(id, zadatak);
        //        return Ok($"Zadatak sa ID-jem {id} je uspešno ažuriran.");
        //    }
        //    catch (NHibernate.ObjectNotFoundException)
        //    {
        //        // Ova greška se desi ako DTOManager.AzurirajZadatak koristi s.Load<T>()
        //        // i ne nađe zadatak. Vraćamo 404 Not Found.
        //        return NotFound($"Zadatak sa ID-jem {id} nije pronađen.");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Sve ostale greške su 500 Internal Server Error
        //        Console.Error.WriteLine(ex.ToString());
        //        return StatusCode(500, "Došlo je do interne greške na serveru.");
        //    }
        //}

        private IActionResult AzurirajZadatak(int id, ZadatakDetalji zadatak)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.AzurirajZadatak(id, zadatak);
                return Ok($"Zadatak sa ID-jem {id} je uspešno ažuriran.");
            }
            catch (NHibernate.ObjectNotFoundException)
            {
                // Greška ako zadatak sa datim ID-jem uopšte ne postoji.
                return NotFound($"Zadatak sa ID-jem {id} nije pronađen.");
            }
            catch (InvalidOperationException ex)
            {
                // Greška ako se tipovi ne poklapaju (iz DTOManager-a).
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Sve ostale, neočekivane greške.
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do interne greške na serveru.");
            }
        }



        #endregion


        [HttpDelete]
        [Route("ObrisiZadatak/{id}")]
        public IActionResult ObrisiZadatak(int id)
        {
            try
            {
                DTOManager.ObrisiZadatak(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }
        public class AngazujUcesnikaRequest
        {
            public int UcesnikId { get; set; }
            public string? TipUcesnika { get; set; } // Očekuje "Stanovnik" ili "Robot"
        }

        #region Upravljanje Vezama

        /// <summary>
        /// Angažuje učesnika (stanovnika ili robota) na određenom zadatku.
        /// </summary>
        /// <param name="zadatakId">ID zadatka.</param>
        /// <param name="request">Podaci o učesniku (ID i tip).</param>
        [HttpPost]
        [Route("{zadatakId}/angazuj-ucesnika")]
        public IActionResult AngazujUcesnikaNaZadatku(int zadatakId, [FromBody] AngazujUcesnikaRequest request)
        {
            if (string.IsNullOrEmpty(request.TipUcesnika) || (request.TipUcesnika != "Stanovnik" && request.TipUcesnika != "Robot"))
            {
                return BadRequest("Tip učesnika mora biti 'Stanovnik' ili 'Robot'.");
            }

            try
            {
                DTOManager.AngazujUcesnikaNaZadatku(request.UcesnikId, request.TipUcesnika, zadatakId);
                return Ok($"Učesnik (Tip: {request.TipUcesnika}, ID: {request.UcesnikId}) je uspešno angažovan na zadatku sa ID {zadatakId}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        #endregion

    }
}
