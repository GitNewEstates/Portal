using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Cors;

namespace API.Controllers
{
    [EnableCors()]
    [ApiController]
  
    public class CasesController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        // GET: CasesController
      

        
        [Route("api/[controller]")]
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }



        
        [Route("api/[controller]/UpdateTitle")]
        [HttpPost]
        public async Task<ActionResult> UpdateTitle([FromBody] Cases _Case)
        {
            await CaseMethods.UpdateTitle(_Case.id);
            return Ok();
        }
       


    }
}
