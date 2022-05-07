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
        private readonly CashMachineContext _сashMachineContext;

        public ATMController(CashMachineContext сashMachineContext)
        {
            _сashMachineContext = сashMachineContext;
        }

        /// <summary>
        /// Retrieve the Cassettes list by their ATMId.
        /// </summary>
        /// <param name="id">ID of ATM</param>
        /// <returns>Cassette list</returns>
        /// <response code="200">OK.Returns the Cassettes list.</response>
        /// <response code="400">Bad Request. ATM not found.</response>
        // GET: api/ATM/<id>/Cassettes
        [HttpGet("{id}/Cassettes")]
        public async Task<ActionResult> GetCassettes(Guid id)
        {
            var list = _сashMachineContext
                .Cassettes
                .Where(
                    cassette => cassette.AtmId == id).ToList()
                .OrderBy(x => x.Value).ToList();

            list.Reverse();

            if (list.Count == 0) return BadRequest("Ошибка. Кассет для данного банкомата не существует.");
            return Ok(list);
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
        public async Task<ActionResult> Get(Guid? id)
        {
            if (id == null)
            {
                return BadRequest("Ошибка ввода.");
            }

            var atm = await _сashMachineContext.ATMs.SingleOrDefaultAsync(x => x.AtmId == id);

            if (atm == null) return BadRequest("Ошибка ввода. Не найден банкомат.");
            return Ok(atm);

            // return _сashMachineContext.ATMs.SingleOrDefault(x => x.AtmId == id);
        }

        /// <summary>
        /// Reduces amount in Cassettes in specific ATM
        /// </summary>
        /// <param name="id">id of ATM</param>
        /// <param name="value">value of money to take</param>
        // POST: api/ATM/<id>/GetMoney/<value>
        [HttpPost("{id}/GetMoney/{value}")]
        public async Task<IActionResult> TakeMoneyFromATM(Guid id, int value)
        {
            var list = _сashMachineContext
                .Cassettes
                .Where(
                    cassette => cassette.AtmId == id).ToList()
                .OrderBy(x => x.Value).ToList();

            list.Reverse();
            var sum = 0;
            
            foreach (var cassette in list)
            {
                while ((cassette.IsWorking.Equals(true) && (cassette.Amount > 0) && (sum + cassette.Value <= value)))
                {
                    cassette.Amount -= 1;
                    sum += cassette.Value;
                }
            }
            
            if (sum == value)
            {
                var atm = await _сashMachineContext.ATMs.SingleOrDefaultAsync(x => x.AtmId == id);
                if (atm == null) return BadRequest("Ошибка ввода. Не найден банкомат.");

                atm.CassetteList = list;

                if (Validation.Validation.CheckCassetteList(atm.CassetteList))
                {
                    _сashMachineContext.ATMs.Update(atm);
                    await _сashMachineContext.SaveChangesAsync();
                    return Ok(atm);
                }
                return BadRequest("Банкомат не обновлен");
            }
            return BadRequest("Ошибка снятия денег");
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

            var list = _сashMachineContext
                .Cassettes
                .Where(
                    cassette => cassette.AtmId == id).ToList()
                .OrderBy(x => x.Value).ToList();

            list.Reverse();
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
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        // POST api/ATM
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ATM atm)
        {
            if (Validation.Validation.CheckCassetteList(atm.CassetteList))
            {
                await _сashMachineContext.ATMs.AddAsync(atm);
                await _сashMachineContext.SaveChangesAsync();
                return Ok(atm);
            }

            return BadRequest("Банкомат не создан");
        }

        /// <summary>
        /// Edit an ATM.
        /// </summary>
        /// <param name="atm">Changable ATM.</param>
        // PUT api/ATM/<id>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] ATM atm)
        {
            if (Validation.Validation.CheckCassetteList(atm.CassetteList))
            {
                _сashMachineContext.ATMs.Update(atm);
                await _сashMachineContext.SaveChangesAsync();
                return Ok(atm);
            }
            return BadRequest("Банкомат не обновлен");
        }

        /// <summary>
        /// Deletes a specific ATM by their id.
        /// </summary>
        /// <param name="id">id of ATM</param>
        // DELETE api/ATM/<id>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = _сashMachineContext.ATMs.SingleOrDefault(x => x.AtmId == id);
            if (item != null)
            {
                _сashMachineContext.ATMs.Remove(item);
                await _сashMachineContext.SaveChangesAsync();
                return Ok("Удаление прошло успешно");
            }
            return BadRequest("Удаление не произошло");
        }
    }
}
