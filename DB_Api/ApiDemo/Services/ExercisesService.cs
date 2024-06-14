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

        public bool getAllExercises(ref List<Exercise> exercises, ref string error)
        {
            if (databaseHelper.getAllExercises(ref exercises, ref error) == true)
            {
                return true;
            }
            return false;
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

        public bool addExercise(Exercise exercise, ref string error)
        {
            if (databaseHelper.addExercise(exercise, ref error) == true)
            {
                return true;
            }
            return false;
        }

        public bool UpdateMovie(ref string error)
        {
            return false;
        }

        public bool DeleteMovie(ref string error)
        {
            return false;
        }
    }
}
