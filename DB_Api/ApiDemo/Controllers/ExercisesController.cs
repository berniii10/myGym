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

        private readonly ExercisesService exercises_service;

        public ExercisesController(ExercisesService _exercisesService)
        {
            this.exercises_service = _exercisesService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Exercise> exercises = new List<Exercise>();
            string error = "";

            if (exercises_service.getAllExercises(ref exercises, ref error) == true)
            {
                return Ok(new { Exercises = exercises, Status = "OK" });
            }
            else
            {
                return NotFound(new { Message = error, Status = "OK" });
            }
        }

        // GET api/exercises/id
        [HttpGet("id/{id}")]
        public IActionResult Get(int id)
        {
            Exercise exercise = new Exercise();
            string error = null;

            if (exercises_service.getExerciseById(id, ref exercise, ref error) == true)
            {
                return Ok(new { Exercise = exercise, Status = "OK" });
            }
            else
            {
                return NotFound(new { Id = id, Message = error, Status = "OK" });
            }
        }

        // GET api/exercises/name
        [HttpGet("name/{name}")]
        public IActionResult Get(string name)
        {
            Exercise exercise = new Exercise();
            string error = null;
            string _name = Uri.UnescapeDataString(name);

            if (exercises_service.getExerciseByName(_name, ref exercise, ref error) == true)
            {
                return Ok(new { Exercise = exercise, Status = "OK" });
            }
            else
            {
                return NotFound(new { Name = name, Message = error, Status = "OK" });
            }
        }

        // POST api/exercises
        [HttpPost]
        public IActionResult Post([FromBody] Exercise exercise)
        {
            string error = "";

            if (exercises_service.addExercise(exercise, ref error) == true)
            {
                return Ok(new { Message = "Exercise Added.", Status = "OK" });
            }
            else
            {
                return Conflict(new { Message = error, ID = exercise.id, Name = exercise.name, ExistingExercise = exercise, Status = "OK" });
            }
        }

        // PUT api/<MoviesController>/
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Exercise exercise)
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
