﻿using ApiDemo.Models;
using ApiDemo.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiDemo.Controllers
{
    [Route("api/exercises/[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {

        private readonly ExercisesService exercisesService;

        public ExercisesController(ExercisesService _exercisesService)
        {
            this.exercisesService = _exercisesService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Exercise> exercises = new List<Exercise>();
            string error = null;

            if (exercisesService.getAllExercises(ref exercises, ref error) == true)
            {
                return Ok(new { Exercises = exercises, Status = "OK" });
            }
            else
            {
                return NotFound(new { Message = error, Status = "OK" });
            }
        }

        // GET api/MoviesController
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Exercise exercise = new Exercise();
            string error = null;

            if (exercisesService.getExercise(id, ref exercise, ref error) == true)
            {
                return Ok(new { Exercise = exercise, Status = "OK" });
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
