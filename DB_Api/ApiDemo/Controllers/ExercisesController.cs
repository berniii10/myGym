using ApiDemo.Models;
using ApiDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiDemo.Controllers
{
    [Route("api/exercises/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {

        public ExercisesController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            Model model = new Model();
            if (true)
            {
                return Ok(new { Model = model, Status = "OK" });
            }
            else
            {
                return NotFound(new { Message = "Movies = null", Status = "OK" });
            }
        }

        // GET api/MoviesController
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Movie movie = new Movie();
            if (true)
            {
                return Ok(new { Movie = movie, Status = "OK" });
            }
            else
            {
                return NotFound(new { Id = id, Message = "Movie not found", Status = "OK" });
            }
        }

        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult Post([FromBody] Movie movie)
        {
            if (true)
            {
                return Ok(new { Message = "Movie Added.", Status = "OK" });
            }
            else
            {
                return Conflict(new { Message = "Movie already exists or there is a movie with that ID.", ID = movie.Id, ExistingMovie = movie, Status = "OK" });
            }
        }

        // PUT api/<MoviesController>/
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Movie movie)
        {
            if (true)
            {
                return Ok(new { Message = "Movie Updated.", Status = "OK" });
            }
            else
            {
                return NotFound(new { Id = id, Message = "Movie not found", Status = "OK" });
            }
        }

        // DELETE api/<MoviesController>/
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (true)
            {
                return Ok(new { Message = "Movie Deleted.", Status = "OK" });
            }
            else
            {
                return NotFound(new { Id = id, Message = "Movie not found", Status = "OK" });
            }
        }
    }
}
