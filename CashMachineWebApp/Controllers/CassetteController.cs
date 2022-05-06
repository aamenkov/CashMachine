using System.Linq;
using System.Threading.Tasks;
using CashMachineWebApp.Context;
using CashMachineWebApp.Models;
using CashMachineWebApp.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CashMachineWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CassetteController : ControllerBase
    {
        private readonly CashMachineContext _сashMachineContext;

        public CassetteController(CashMachineContext сashMachineContext)
        {
            _сashMachineContext = сashMachineContext;
        }

        /// <summary>
        /// Retrieve the Cassette by their id.
        /// </summary>
        /// <param name="id">id of Cassette</param>
        /// <returns>Cassette</returns>
        /// <response code="200">Returns the Cassette</response>
        /// <response code="204">Cassette not found</response>
        // GET: api/Cassette/<id>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int ?id)
        {
            if (id == null)
            {
                return BadRequest("Ошибка");
            }

            var cassette = await _сashMachineContext.Cassettes.SingleOrDefaultAsync(x => x.CassetteId == id);

            if (cassette == null) return BadRequest("Ошибка ввода");
            return Ok(cassette);
        }

        /// <summary>
        /// Creates a new Cassette.
        /// </summary>
        /// <param name="cassette"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        // POST: api/Cassette
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cassette cassette)
        {
            if (Validation.Validation.CheckCassette(cassette))
            {
                await _сashMachineContext.Cassettes.AddAsync(cassette);
                await _сashMachineContext.SaveChangesAsync();
                return Ok(cassette);
            }
            return BadRequest("Кассета не создана");
        }

        /// <summary>
        /// Edit an Cassette.
        /// </summary>
        /// <param name="cassette">Changable Cassette.</param>
        // PUT: api/Cassette/<id>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] Cassette cassette)
        {
            if (Validation.Validation.CheckCassette(cassette)){
                _сashMachineContext.Cassettes.Update(cassette);
                await _сashMachineContext.SaveChangesAsync();
                return Ok(cassette);
            }
            return BadRequest("Кассета не обновлена");
        }

        /// <summary>
        /// Deletes a specific Cassette by their id.
        /// </summary>
        /// <param name="id">id of Cassette</param>
        // DELETE: api/Cassette/<id>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = _сashMachineContext.Cassettes.SingleOrDefault(x => x.CassetteId == id);
            if (item != null)
            {
                _сashMachineContext.Cassettes.Remove(item);
                await _сashMachineContext.SaveChangesAsync();
                return Ok("Удаление прошло успешно");
            }
            return BadRequest("Удаление не произошло");
        }
    }
}
