using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Data;
using QuotesApi.Models;

namespace QuotesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : ControllerBase
    {
        QuotesDbContext _quotesDbContext;

        public QuotesController(QuotesDbContext quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        // GET: api/Quotes
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_quotesDbContext.Quoets);
        }

        // GET: api/Quotes/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var quote = _quotesDbContext.Quoets.Find(id);

            if (quote == null)
            {
                return NotFound("No record found against this id.");
            }
            else
            {
                return Ok(quote);
            }

        }

        [HttpGet("[action]")]
        public ActionResult PagingQuote(int? pageNumber, int? pageSize)
        {
            var quotes = _quotesDbContext.Quoets;
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;

            return Ok(quotes.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));

        }

        [HttpGet("[action]")]
        public ActionResult SearchQuote(string type)
        {
            var quotes = _quotesDbContext.Quoets.Where(q => q.Type.StartsWith(type));
            return Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult MyQuote()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var quotes = _quotesDbContext.Quoets.Where(q => q.UserId == userId);
            return Ok(quotes);
        }

        // POST: api/Quotes
        [HttpPost]
        public IActionResult Post([FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;

            _quotesDbContext.Quoets.Add(quote);
            _quotesDbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var entity = _quotesDbContext.Quoets.Find(id);

            if (entity == null)
            {
                return NotFound("No record found against this id.");
            }

            if (userId != entity.UserId)
            {
                return BadRequest("Sorry you can't update this record....");
            }
            else
            {
                entity.Title = quote.Title;
                entity.Author = quote.Author;
                entity.Description = quote.Description;
                entity.Type = quote.Type;
                entity.CreatedAt = quote.CreatedAt;

                _quotesDbContext.SaveChanges();

                return Ok("Record Updated Successfully.");
            }

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var quote = _quotesDbContext.Quoets.Find(id);

            if (quote == null)
            {
                return NotFound("No record found against this id.");
            }

            if (userId != quote.UserId)
            {
                return BadRequest("Sorry you can't update this record....");
            }
            else
            {
                _quotesDbContext.Quoets.Remove(quote);
                _quotesDbContext.SaveChanges();

                return Ok("Quote deleted.");

            }

        }

        [HttpGet("[action]")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public ActionResult sortQuote(string sort)
        {
            IQueryable<Quote> quotes;
            switch (sort)
            {
                case "desc":
                    quotes = _quotesDbContext.Quoets.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    quotes = _quotesDbContext.Quoets.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    quotes = _quotesDbContext.Quoets;
                    break;
            }

            return Ok(quotes);
        }
    }
}
