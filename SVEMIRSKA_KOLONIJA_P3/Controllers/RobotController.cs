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
    public class RobotController : ControllerBase
    {
        [HttpGet]
        [Route("VratiSveRobote")]
        public IActionResult VratiSveRobote()
        {
            try
            {
                return Ok(DTOManager.VratiSveRobote());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpGet]
        [Route("VratiRobota/{id}")]
        public IActionResult VratiRobota(int id)
        {
            try
            {
                var robot = DTOManager.VratiRobotaDetalji(id);
                if (robot == null)
                {
                    return NotFound($"Robot sa ID-jem {id} nije pronađen.");
                }
                return Ok(robot);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return StatusCode(500, "Došlo je do greške na serveru.");
            }
        }

        [HttpPost]
        [Route("DodajRobota")]
        public IActionResult DodajRobota([FromBody] RobotDetalji robot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.DodajRobota(robot);
                return StatusCode(201, "Robot je uspešno kreiran."); // Vraćamo 201 Created
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("AzurirajRobota")]
        public IActionResult AzurirajRobota([FromBody] RobotDetalji robot)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                DTOManager.AzurirajRobota(robot);
                return Ok("Robot je uspešno ažuriran.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("ObrisiRobota/{id}")]
        public IActionResult ObrisiRobota(int id)
        {
            try
            {
                DTOManager.ObrisiRobota(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške na serveru: {ex.Message}");
            }
        }
    }

}
