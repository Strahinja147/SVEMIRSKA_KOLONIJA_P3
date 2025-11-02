using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SVEMIRSKA_KOLONIJA_P3; // Obavezno dodati using za namespace gde je DTOManager
using SVEMIRSKA_KOLONIJA_P3.DTOs;
using SVEMIRSKA_KOLONIJA;



namespace SVEMIRSKA_KOLONIJA_P3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResursController : ControllerBase
    {
        [HttpGet]
        [Route("VratiSveResurse")]
        public IActionResult VratiSveResurse()
        {
            try
            {
                return Ok(DTOManager.VratiSveResurse());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpGet]
        [Route("VratiResurs/{id}")]
        public IActionResult VratiResurs(int id)
        {
            try
            {
                var resurs = DTOManager.VratiResursDetalji(id);
                if (resurs == null)
                {
                    return NotFound($"Resurs sa ID-jem {id} nije pronađen.");
                }
                return Ok(resurs);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpPost]
        [Route("DodajResurs")]
        public IActionResult DodajResurs([FromBody] ResursDetalji resurs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Pozivamo DTOManager koji sada vraća kreirani 'Resurs' objekat
                var noviResurs = DTOManager.DodajResurs(resurs);

                // Vraćamo '201 Created' status, URL do novog resursa, i sam objekat
                // Pretpostavka je da imate metodu 'VratiResursDetalji' u ovom kontroleru
                return CreatedAtAction(nameof(VratiResurs), new { id = noviResurs.Id }, noviResurs);
            }
            catch (Exception ex)
            {
                // Logujemo grešku i vraćamo '500 Internal Server Error'
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do interne greške na serveru.");
            }
        }

        [HttpPut]
        [Route("AzurirajResurs")]
        public IActionResult AzurirajResurs([FromBody] ResursDetalji resurs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.AzurirajResurs(resurs);
                return Ok("Resurs je uspešno ažuriran.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("ObrisiResurs/{id}")]
        public IActionResult ObrisiResurs(int id)
        {
            try
            {
                DTOManager.ObrisiResurs(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        #region Upravljanje Vezama

        /// <summary>
        /// Dodeljuje postojećeg stanovnika kao upravitelja resursu.
        /// </summary>
        /// <param name="resursId">ID resursa.</param>
        /// <param name="stanovnikId">ID stanovnika koji postaje upravitelj.</param>
        [HttpPost]
        [Route("{resursId}/upravitelji/{stanovnikId}")]
        public IActionResult DodeliUpraviteljaResursu(int resursId, int stanovnikId)
        {
            try
            {
                bool uspeh = DTOManager.DodeliUpraviteljaResursu(stanovnikId, resursId);
                if (uspeh)
                {
                    return Ok($"Stanovnik sa ID {stanovnikId} je uspešno postavljen za upravitelja resursa sa ID {resursId}.");
                }
                return BadRequest("Neuspešna dodela upravitelja.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        #endregion
    }

}
