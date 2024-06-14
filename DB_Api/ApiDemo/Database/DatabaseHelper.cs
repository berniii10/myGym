using ApiDemo.Models;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Xml.Linq;

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
        public DataTable? ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

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
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }

            return dataTable;
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
                            error = "No instructions found for the given exercise ID.";
                            return false;
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
    }
}
