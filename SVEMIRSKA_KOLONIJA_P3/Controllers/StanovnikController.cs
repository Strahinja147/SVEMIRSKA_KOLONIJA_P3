using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SVEMIRSKA_KOLONIJA_P3; 
using SVEMIRSKA_KOLONIJA_P3.DTOs;
using SVEMIRSKA_KOLONIJA;


namespace SVEMIRSKA_KOLONIJA_P3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StanovnikController : ControllerBase
    {
        [HttpGet]
        [Route("VratiSveStanovnike")]
        public IActionResult VratiSveStanovnike()
        {
            try
            {
                return Ok(DTOManager.VratiSveStanovnike());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpGet]
        [Route("VratiStanovnika/{id}")]
        public IActionResult VratiStanovnika(int id)
        {
            try
            {
                var stanovnik = DTOManager.VratiStanovnikaDetalji(id);
                if (stanovnik == null)
                {
                    return NotFound($"Stanovnik sa ID-jem {id} nije pronađen.");
                }
                return Ok(stanovnik);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpPost]
        [Route("DodajStanovnika")]
        public IActionResult DodajStanovnika([FromBody] StanovnikDetalji stanovnik)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var noviStanovnik = DTOManager.DodajStanovnika(stanovnik);
                if (noviStanovnik == null)
                {
                    return BadRequest("Neuspešno dodavanje stanovnika.");
                }
                // Vraća novokreiranog stanovnika sa njegovim ID-jem
                return CreatedAtAction(nameof(VratiStanovnika), new { id = noviStanovnik.Id }, noviStanovnik);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("AzurirajStanovnika")]
        public IActionResult AzurirajStanovnika([FromBody] StanovnikDetalji stanovnik)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                DTOManager.AzurirajStanovnika(stanovnik);
                return Ok("Stanovnik je uspešno ažuriran.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("ObrisiStanovnika/{id}")]
        public IActionResult ObrisiStanovnika(int id)
        {
            try
            {
                DTOManager.ObrisiStanovnika(id);
                return NoContent(); // 204 No Content je standard za uspešno brisanje
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru. Proverite da li je stanovnik vođa nekog sektora. Originalna greška: {ex.Message}");
            }
        }

        #region Upravljanje Vezama

        /// <summary>
        /// Dodeljuje novu specijalizaciju stanovniku.
        /// </summary>
        /// <param name="stanovnikId">ID stanovnika.</param>
        /// <param name="specijalizacijaId">ID specijalizacije koja se dodeljuje.</param>
        /// <param name="posedujeDetalji">Dodatni detalji o specijalizaciji (nivo, datum, institucija).</param>
        [HttpPost]
        [Route("{stanovnikId}/specijalizacije/{specijalizacijaId}")]
        public IActionResult DodajSpecijalizacijuStanovniku(int stanovnikId, int specijalizacijaId, [FromBody] PosedujePregled posedujeDetalji)
        {
            try
            {
                DTOManager.DodajSpecijalizacijuStanovniku(stanovnikId, specijalizacijaId, posedujeDetalji);
                return StatusCode(201, "Specijalizacija je uspešno dodeljena stanovniku.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        /// <summary>
        /// Briše vezu između stanovnika i specijalizacije.
        /// </summary>
        /// <param name="posedujeId">ID veze 'Poseduje' koja se briše.</param>
        [HttpDelete]
        [Route("poseduje-vezu/{posedujeId}")]
        public IActionResult ObrisiPosedujeVezu(int posedujeId)
        {
            try
            {
                DTOManager.ObrisiPosedujeVezu(posedujeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        /// <summary>
        /// Dodaje novi kontakt na Zemlji za određenog stanovnika.
        /// </summary>
        /// <param name="stanovnikId">ID stanovnika za kojeg se dodaje kontakt.</param>
        /// <param name="kontakt">Podaci o kontaktu.</param>
        [HttpPost]
        [Route("{stanovnikId}/kontakti")]
        public IActionResult DodajKontaktZaStanovnika(int stanovnikId, [FromBody] KontaktNaZemljiPregled kontakt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.DodajKontaktZaStanovnika(stanovnikId, kontakt);
                return StatusCode(201, "Kontakt je uspešno dodat.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        /// <summary>
        /// Briše kontakt na Zemlji.
        /// </summary>
        /// <param name="kontaktId">ID kontakta koji se briše.</param>
        [HttpDelete]
        [Route("kontakti/{kontaktId}")]
        public IActionResult ObrisiKontakt(int kontaktId)
        {
            try
            {
                DTOManager.ObrisiKontakt(kontaktId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        #endregion
    }
}
