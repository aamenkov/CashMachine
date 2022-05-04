using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using CashMachineWebApp.Context;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CashMachineWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CashMachineWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ATMController : ControllerBase
    {
        private readonly CRUDContext _CRUDContext;

        public ATMController(CRUDContext crudContext)
        {
            _CRUDContext = crudContext;
        }

        /// <summary>
        /// Retrieve the Cassettes list by their ATMId.
        /// </summary>
        /// <param name="id">ID of ATM</param>
        /// <returns>Cassette list</returns>
        /// <response code="200">Returns the Cassettes list.</response>
        /// <response code="204">ATM not found.</response>
        // GET: api/ATM/<id>/Cassettes
        [HttpGet("{id}/Cassettes")]
        public List<Cassette> GetCassettes(Guid id)
        {
            var list = _CRUDContext.Cassettes.Where(cassette => cassette.AtmId == id).ToList().OrderBy(x => x.Value).ToList();
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Retrieve the ATM by their id.
        /// </summary>
        /// <param name="id">id of ATM</param>
        /// <returns>ATM</returns>
        /// <response code="200">Returns the Cassette</response>
        /// <response code="204">Cassette not found</response>
        // GET api/ATM/<id>
        [HttpGet("{id}")]
        public ATM Get(Guid id)
        {
            return _CRUDContext.ATMs.SingleOrDefault(x => x.AtmId == id);
        }

        /// <summary>
        /// Reduces amount in Cassettes in specific ATM
        /// </summary>
        /// <param name="id">id of ATM</param>
        /// <param name="value">value of money to take</param>
        // POST: api/ATM/<id>/GetMoney/<value>
        [HttpPost("{id}/GetMoney/{value}")]
        public void TakeMoneyFromATM(Guid id, int value)
        {
            var list = GetCassettes(id);
            var sum = 0;
            
            foreach (var cassette in list)
            {
                while ((cassette.IsWorking.Equals(true) && (cassette.Amount > 0) && (sum + cassette.Value <= value)))
                {
                    cassette.Amount = cassette.Amount - 1;
                    sum = sum + cassette.Value;
                }
            }
            
            if (sum == value)
            {
                var atm = Get(id);
                atm.CassetteList = list;
                Put(atm);
            }
        }

        /// <summary>
        /// Checks a specific ATM for dispensing money
        /// </summary>
        /// <param name="id">id of ATM</param>
        /// <param name="value">value of money to take</param>
        /// <returns></returns>
        // GET: api/ATM/<id>/GetMoney/<value>
        [HttpGet("{id}/CheckToGetMoney/{value}")]
        public DTOServerResponse CheckToGetMoney(Guid id, int value)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var list = GetCassettes(id);
            var sum = 0;
            var dictionary = new ConcurrentDictionary<int, int>();

            foreach (var cassette in list)
            {
                while ((cassette.IsWorking.Equals(true) && (cassette.Amount > 0) && (sum + cassette.Value <= value)))
                {
                    cassette.Amount -= 1;
                    sum += cassette.Value;

                    if (dictionary.TryGetValue(cassette.Value, out var val))
                    {
                        dictionary[cassette.Value] = val + 1;
                    }
                    else
                    {
                        dictionary.TryAdd(cassette.Value, 1);
                    }
                }
            }

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            if (sum != value)
            {
                return new DTOServerResponse("NOT OK", "RunTime " + elapsedTime, null);
            }
            else
            {
                return new DTOServerResponse("OK", "RunTime " + elapsedTime, dictionary);
            }
        }

        /// <summary>
        /// Creates an new ATM.
        /// </summary>
        /// <param name="atm">new ATM</param>
        // POST api/ATM
        [HttpPost]
        public void Post([FromBody] ATM atm)
        {
            if (Validation.Validation.CheckCassetteList(atm.CassetteList))
            {
                _CRUDContext.ATMs.Add(atm);
                _CRUDContext.SaveChanges();
            }
        }

        /// <summary>
        /// Edit an ATM.
        /// </summary>
        /// <param name="atm">Changable ATM.</param>
        // PUT api/ATM/<id>
        [HttpPut("{id}")]
        public void Put([FromBody] ATM atm)
        {
            if (Validation.Validation.CheckCassetteList(atm.CassetteList))
            {
                _CRUDContext.ATMs.Update(atm);
                _CRUDContext.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a specific ATM by their id.
        /// </summary>
        /// <param name="id">id of ATM</param>
        // DELETE api/ATM/<id>
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            var item = _CRUDContext.ATMs.SingleOrDefault(x => x.AtmId == id);
            if (item != null)
            {
                _CRUDContext.ATMs.Remove(item);
                _CRUDContext.SaveChanges();
            }
        }
    }
}
