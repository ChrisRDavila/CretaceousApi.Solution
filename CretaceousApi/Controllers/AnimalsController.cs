using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CretaceousApi.Models;
using System;

namespace CretaceousApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AnimalsController : ControllerBase
  {
    private readonly CretaceousApiContext _db;

    public AnimalsController(CretaceousApiContext db)
    {
      _db = db;
    }

    [HttpGet("page/{page}")]
    public async Task<ActionResult<List<Animal>>> GetPages(int page, int pageSize = 4)
    {
        if (_db.Animals == null)
        return NotFound();

      int pageCount = _db.Animals.Count();

      var animals = await _db.Animals
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        var response = new Response
      {
        Animals = animals,
        //page number inside the url
        CurrentPage = page,
        //the amount of animals returned from the database
        Pages = pageCount,
        //amnt of items on the page
        PageSize = pageSize
      };
      return Ok(response);
    }

    // GET api/animals
    [HttpGet]
    public async Task<List<Animal>> Get(string species, string name, int minimumAge)
    {
      IQueryable<Animal> query = _db.Animals.AsQueryable();

      if (species != null)
      {
        query = query.Where(entry => entry.Species == species);
      }

      if (name != null)
      {
        query = query.Where(entry => entry.Name == name);
      }

      if (minimumAge > 0)
      {
        query = query.Where(entry => entry.Age >= minimumAge);
      }

      return await query.ToListAsync();
    }

    // GET: api/Animals/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(int id)
    {
      Animal animal = await _db.Animals.FindAsync(id);

      if (animal == null)
      {
        return NotFound();
      }

      return animal;
    }

    // POST api/animals
    [HttpPost]
    public async Task<ActionResult<Animal>> Post([FromBody] Animal animal)
    {
      _db.Animals.Add(animal);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(GetAnimal), new { id = animal.AnimalId }, animal);
    }

        // PUT: api/Animals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Animal animal)
    {
      if (id != animal.AnimalId)
      {
        return BadRequest();
      }

      _db.Animals.Update(animal);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!AnimalExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    private bool AnimalExists(int id)
    {
      return _db.Animals.Any(e => e.AnimalId == id);
    }

    // DELETE: api/Animals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
      Animal animal = await _db.Animals.FindAsync(id);
      if (animal == null)
      {
        return NotFound();
      }

      _db.Animals.Remove(animal);
      await _db.SaveChangesAsync();

      return NoContent();
    }

    [HttpGet("random")]
    public async Task<ActionResult<Animal>> RandomAnimal()
    {
      int animals = await _db.Animals.CountAsync();
      
      if (animals == 0)
        {
          return NotFound();
        }

      var random = new Random();
      int randomInt = random.Next(0, animals);

      Animal randomAnimal = await _db.Animals
        .OrderBy(animal => animal.AnimalId)
        .Skip(randomInt)
        .FirstOrDefaultAsync();

      return Ok(randomAnimal);
    }
  }
}
