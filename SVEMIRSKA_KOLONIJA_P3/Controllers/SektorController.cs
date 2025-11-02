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
    public class SektorController : ControllerBase
    {
        [HttpGet]
        [Route("VratiSveSektore")]
        public IActionResult VratiSveSektore()
        {
            try
            {
                return Ok(DTOManager.VratiSveSektore());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpGet]
        [Route("VratiSektor/{id}")]
        public IActionResult VratiSektor(int id)
        {
            try
            {
                var sektor = DTOManager.VratiSektorDetalji(id);
                if (sektor == null)
                {
                    return NotFound($"Sektor sa ID-jem {id} nije pronađen.");
                }
                return Ok(sektor);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpPost]
        [Route("DodajSektor")]
        public IActionResult DodajSektor([FromBody] SektorDetalji sektor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var noviSektor = DTOManager.DodajSektor(sektor);
                if (noviSektor != null)
                {
                    return CreatedAtAction(nameof(VratiSektor), new { id = noviSektor.Id }, noviSektor);
                }
                return BadRequest("Neuspešno dodavanje sektora.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("AzurirajSektor")]
        public IActionResult AzurirajSektor([FromBody] SektorDetalji sektor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.AzurirajSektor(sektor);
                return Ok("Sektor je uspešno ažuriran.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("ObrisiSektor/{id}")]
        public IActionResult ObrisiSektor(int id)
        {
            try
            {
                // 1. Pozivamo metodu. Ne očekujemo povratnu vrednost.
                // Ako se ova linija izvrši bez greške, smatramo da je operacija uspela.
                DTOManager.ObrisiSektor(id);

                // 2. Vraćamo HTTP 204 No Content, što je standard za uspešno brisanje.
                return NoContent();
            }
            catch (Exception ex)
            {
                // 3. Ako je DTOManager bacio grešku, hvatamo je ovde.
                // Logujemo grešku i vraćamo HTTP 500 Internal Server Error.
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do interne greške na serveru. Moguće je da sektor ima zavisne entitete.");
            }
        }

        #region Upravljanje Vezama

        /// <summary>
        /// Dodeljuje postojećeg radnika postojećem sektoru (kreira M:N vezu).
        /// </summary>
        /// <param name="sektorId">ID sektora kojem se dodeljuje radnik.</param>
        /// <param name="radnikId">ID stanovnika (radnika) koji se dodeljuje.</param>
        [HttpPost]
        [Route("{sektorId}/radnici/{radnikId}")]
        public IActionResult DodeliRadnikaSektoru(int sektorId, int radnikId)
        {
            try
            {
                // 1. Pozivamo void metodu. Ne hvatamo povratnu vrednost jer je nema.
                // Ako se ne desi greška, znamo da je operacija uspela.
                DTOManager.DodeliRadnikaSektoru(radnikId, sektorId);

                // 2. Ako nema greške, vraćamo 200 OK sa porukom o uspehu.
                // Ovo je sada mnogo jednostavnije.
                return Ok($"Radnik sa ID {radnikId} je uspešno dodeljen sektoru sa ID {sektorId}.");
            }
            catch (Exception ex)
            {
                // 3. Catch blok sada ispravno hvata greške iz DTOManagera 
                //    (npr. ako radnik ili sektor sa tim ID-jem ne postoje).
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do interne greške na serveru.");
            }
        }

        /// <summary>
        /// Uklanja radnika iz sektora (briše M:N vezu).
        /// </summary>
        /// <param name="sektorId">ID sektora.</param>
        /// <param name="radnikId">ID radnika.</param>
        [HttpDelete]
        [Route("{sektorId}/radnici/{radnikId}")]
        public IActionResult UkloniRadnikaIzSektora(int sektorId, int radnikId)
        {
            try
            {
                DTOManager.UkloniRadnikaIzSektora(radnikId, sektorId);
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
