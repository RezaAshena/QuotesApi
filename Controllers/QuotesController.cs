using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Models;

namespace QuotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotesController : ControllerBase
    {
        static List<Quote> _qoutes = new List<Quote>()
        {
            new Quote(){Id=0,Author="Reza Ashena",Description="description 1",Title="title one"},
            new Quote(){Id=1,Author="John Smith",Description="description 2",Title="title two"}
        };

        [HttpGet]
        public IEnumerable<Quote> Get()
        {
            return _qoutes;
        }

        [HttpPost]
        public void Post([FromBody]Quote quote)
        {
            _qoutes.Add(quote);
        }

        [HttpPut("{id}")]
        public void Put(int id,[FromBody]Quote quote)
        {
            _qoutes[id] = quote;
        }

        [HttpDelete("{id}")]
        public void Delete (int id)
        {
            _qoutes.RemoveAt(id);
        }
    }
}