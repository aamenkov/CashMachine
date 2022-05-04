using System.Linq;
using CashMachineWebApp.Context;
using CashMachineWebApp.Models;
using CashMachineWebApp.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CashMachineWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CassetteController : ControllerBase
    {
        private readonly CRUDContext _CRUDContext;

        public CassetteController(CRUDContext crudContext)
        {
            _CRUDContext = crudContext;
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
        public Cassette Get(int id)
        {
            return _CRUDContext.Cassettes.SingleOrDefault(x=>x.CassetteId == id);
        }

        /// <summary>
        /// Creates a new Cassette.
        /// </summary>
        /// <param name="cassette"></param>
        // POST: api/Cassette
        [HttpPost]
        public void Post([FromBody] Cassette cassette)
        {
            if (Validation.Validation.CheckCassette(cassette))
            {
                _CRUDContext.Cassettes.Add(cassette);
                _CRUDContext.SaveChanges();
            }

        }

        /// <summary>
        /// Edit an Cassette.
        /// </summary>
        /// <param name="cassette">Changable Cassette.</param>
        // PUT: api/Cassette/<id>
        [HttpPut("{id}")]
        public void Put([FromBody] Cassette cassette)
        {
            if (Validation.Validation.CheckCassette(cassette)){
                _CRUDContext.Cassettes.Update(cassette);
                _CRUDContext.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a specific Cassette by their id.
        /// </summary>
        /// <param name="id">id of Cassette</param>
        // DELETE: api/Cassette/<id>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var item = _CRUDContext.Cassettes.SingleOrDefault(x => x.CassetteId == id);
            if (item != null)
            {
                _CRUDContext.Cassettes.Remove(item);
                _CRUDContext.SaveChanges();
            }
        }

    }
}
