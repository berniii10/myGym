using ApiDemo.Database;
using ApiDemo.Models;

namespace ApiDemo.Services
{
    public class ExercisesService
    {
        private readonly DatabaseHelper databaseHelper;

        public ExercisesService(DatabaseHelper _databaseHelper)
        {
            databaseHelper = _databaseHelper;
        }

        public bool getAllExercises(ref List<Exercise> exercises, ref string? error)
        {
            return true;
        }

        public bool getExerciseById(int id, ref Exercise exercise, ref string error)
        {
            if (databaseHelper.getExercise(id, null, ref exercise, ref error) == true)
            {
                return true;
            }
            return false;
        }

        public bool getExerciseByName(string name, ref Exercise exercise, ref string error)
        {
            if (databaseHelper.getExercise(null, name, ref exercise, ref error) == true)
            {
                return true;
            }
            return false;
        }

        public bool AddMovie()
        {
            return true;
        }

        public bool UpdateMovie(int id)
        {
            return false;
        }

        public bool DeleteMovie(int id)
        {
            return false;
        }
    }
}
