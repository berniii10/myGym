using ApiDemo.Common;
using ApiDemo.Models;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiDemo.Database
{
    public class DatabaseHelper
    {
        // Database connection string
        private readonly string _connectionString;

        // SqlConnection object
        private NpgsqlConnection connection;

        // Constructor to initialize the parameters
        public DatabaseHelper()
        {
            _connectionString = File.ReadAllText("Database/db_config.cf");

            Connect();
        }

        // Method to establish a connection to the database
        public bool Connect()
        {
            if (connection == null)
            {
                connection = new NpgsqlConnection(this._connectionString);
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
                Console.WriteLine("Connection opened successfully.");
                return true;
            }

            return false;
        }

        // Method to disconnect from the database
        public bool Disconnect()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Connection closed successfully.");
                return true;
            }

            return false;
        }

        // Method to execute a generic query
        private bool executeQuery(string query, ref DataTable result_table, ref string error)
        {
            try
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be open to execute queries.");
                }

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(command))
                    {
                        adapter.Fill(result_table);
                    }
                }
            }
            catch (Exception ex)
            {
                error = $"An error occurred: {ex.Message}";
                return false;
            }

            return true;
        }

        public bool getAllExercisesIds(ref List<int> ids, ref string error)
        {
            DataTable result_table = new DataTable();

            // Define the query
            string query = @"SELECT id from exercises e;";
            
            if (executeQuery(query, ref result_table, ref error) == true && result_table != null)
            {
                foreach (DataRow row in result_table.Rows)
                {
                    ids.Add((int)row["id"]);
                }

                return true;
            }

            return false;
        }

        public bool getAllExercises(ref List<Exercise> exercises, ref string error)
        {
            Exercise exercise = new Exercise();
            List<int> ids = new List<int>();
            exercises = new List<Exercise>();

            if (getAllExercisesIds(ref ids, ref error) == true)
            {
                foreach (int id in ids)
                {
                    if (getExercise(id, null, ref exercise, ref error) == true)
                    {
                        exercises.Add(exercise);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool getExercise(int? id, string? name, ref Exercise exercise, ref string error)
        {
            try
            {
                List<Muscle> muscles = new List<Muscle>();

                // Ensure the connection is open
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    error = "Connection must be open to execute queries.";
                    return false;
                }

                // Define the query
                string query = @"
                    SELECT id, name, force, level, mechanic, equipment, category, image_url
                    FROM exercises
                    WHERE id = @exerciseSelector";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    if (id != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", id);
                    }
                    else if (name != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", name);
                    }
                    else
                    {
                        error = "Both id and name are null";
                        return false;
                    }
                    
                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            error = "There are no rows for this exercise ID";
                            return false;
                        }

                        // Process the result set
                        while (reader.Read())
                        {
                            // Initialize the exercise object if it hasn't been initialized
                            if (exercise == null)
                            {
                                exercise = new Exercise();
                            }

                            exercise.id = reader.GetInt32(0);
                            exercise.name = reader.GetString(1);
                            exercise.force = reader.IsDBNull(2) ? null : reader.GetString(2);
                            exercise.level = reader.IsDBNull(3) ? null : reader.GetString(3);
                            exercise.mechanic = reader.IsDBNull(4) ? null : reader.GetString(4);
                            exercise.equipment = reader.IsDBNull(5) ? null : reader.GetString(5);
                            exercise.category = reader.IsDBNull(6) ? null : reader.GetString(6);
                            exercise.image_url = reader.IsDBNull(7) ? null : reader.GetString(7);
                        }
                    }
                }

                string query_for_muscles = @"
                    SELECT m.id, m.name, em.type
                    FROM exercisemuscles em JOIN muscles m ON em.muscle_id = m.id
                    WHERE em.exercise_id = @exerciseSelector;";

                using (var command = new NpgsqlCommand(query_for_muscles, connection))
                {
                    if (id != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", id);
                    }
                    else if (name != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", name);
                    }
                    else
                    {
                        error = "Both id and name are null";
                        return false;
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            error = "No muscles found for the given exercise ID.";
                            return false;
                        }

                        while (reader.Read())
                        {
                            Muscle muscle = new Muscle
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Type = reader.GetString(2)
                            };

                            muscles.Add(muscle);
                        }
                    }
                }

                // Save primary and secondary muscles into the Exercise
                foreach (Muscle muscle in muscles)
                {
                    if (muscle.Type.Equals("primary"))
                    {
                        exercise.primary_muscles.Add(muscle);
                    }
                    else if (muscle.Type.Equals("secondary"))
                    {
                        exercise.secondary_muscles.Add(muscle);
                    }
                }

                string query_for_instructions = @"
                    SELECT step_number, description
                        FROM instructions
                        WHERE exercise_id = @exerciseSelector
                        ORDER BY step_number;";

                using (var command = new NpgsqlCommand(query_for_instructions, connection))
                {
                    if (id != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", id);
                    }
                    else if (name != null)
                    {
                        // Add parameter to the command
                        command.Parameters.AddWithValue("@exerciseSelector", name);
                    }
                    else
                    {
                        error = "Both id and name are null";
                        return false;
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            // Should do something if there are no instructions?
                            //error = "No instructions found for the given exercise ID.";
                            //return false;
                        }

                        while (reader.Read())
                        {
                            Instruction instruction = new Instruction
                            {
                                StepNumber = reader.GetInt32(0),
                                Description = reader.GetString(1),
                            };

                            exercise.instructions.Add(instruction);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                error = ex.Message;
                return false;
            }

            return true;
        }

        public bool addExercise(Exercise exercise, ref string error)
        {
            DataTable result_table = new DataTable();

            int exercise_id;

            string query_to_check_if_exercise = $@"SELECT id FROM Exercises WHERE LOWER(name) = LOWER('{exercise.name}')";

            if (executeQuery(query_to_check_if_exercise, ref result_table, ref error) == true)
            {
                if (result_table.Rows.Count > 0)
                {
                    error = "Exercise already exists";
                    return false;
                }

                string query_for_muscle;

                // Validate that the muscle exists
                foreach (var muscle in exercise.primary_muscles)
                {
                    query_for_muscle = $@"SELECT * FROM muscles WHERE id = {muscle.Id}";
                    result_table.Rows.Clear();
                    
                    if (executeQuery(query_for_muscle, ref result_table, ref error) == true)
                    {
                        if (result_table.Rows.Count == 0)
                        {
                            error = "Primary muscle does not exist, create it before adding the exercise";
                            return false;
                        }
                    } else return false;
                }

                // Validate that the muscle exist
                foreach (var muscle in exercise.secondary_muscles)
                {
                    query_for_muscle = $@"SELECT * FROM muscles WHERE id = {muscle.Id}";
                    result_table.Rows.Clear();

                    if (executeQuery(query_for_muscle, ref result_table, ref error) == true)
                    {
                        if (result_table.Rows.Count == 0)
                        {
                            error = "Secondary muscle does not exist, create it before adding the exercise";
                            return false;
                        }
                    } else return false;
                }

                List<int> ids = new List<int>();
                if (getAllExercisesIds(ref ids, ref error) == false) return false;

                int id = CommonFunctions.getUniqueId(ids);
                // We can now insert the exercise
                string query_add_exercise = $@"
                        INSERT INTO Exercises (id, name, force, level, mechanic, equipment, category, image_url)
                        VALUES ({id}, '{exercise.name}', '{exercise.force}', '{exercise.level}', '{exercise.mechanic}', '{exercise.equipment}', '{exercise.category}', '{exercise.image_url}')
                        RETURNING id";
                result_table.Rows.Clear();

                if (executeQuery(query_add_exercise, ref result_table, ref error) == true)
                {
                    DataRow row = result_table.Rows[0];
                    exercise_id = (int)row["id"];

                    // Loop through all the muscles
                    foreach (var muscle in exercise.primary_muscles)
                    {
                        string query_to_add_muscle = $@"
                            INSERT INTO ExerciseMuscles (exercise_id, muscle_id, type)
                            VALUES ({exercise_id}, {muscle.Id}, 'primary')";
                        result_table.Rows.Clear();

                        if (executeQuery(query_to_add_muscle, ref result_table, ref error) == false)
                        {
                            return false;
                        }
                    }

                    foreach (var muscle in exercise.secondary_muscles)
                    {
                        string query_to_add_muscle = $@"
                            INSERT INTO ExerciseMuscles (exercise_id, muscle_id, type)
                            VALUES ({exercise_id}, {muscle.Id}, 'secondary')";
                        result_table.Rows.Clear();

                        if (executeQuery(query_to_add_muscle, ref result_table, ref error) == false)
                        {
                            return false;
                        }
                    }

                    int step = 0;
                    foreach (var instruction in exercise.instructions)
                    {
                        string query_for_instructions = $@"
                            INSERT INTO Instructions(exercise_id, step_number, description)
                            VALUES({exercise_id}, '{step.ToString()}', '{instruction}');";
                        result_table.Rows.Clear();

                        if (executeQuery(query_for_instructions, ref result_table, ref error) == false)
                        {
                            return false;
                        }

                        step++;
                    }

                } else return false;
            } else return false;
        
            return true;
        }
    }
}
