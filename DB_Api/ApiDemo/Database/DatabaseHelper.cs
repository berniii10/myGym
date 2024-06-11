using ApiDemo.Models;
using Npgsql;
using System.Data;
using System.Data.SqlClient;

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

        public getExercise(int id, ref Exercise exercise, ref string error)
        {
            try
            {
                if (connection == null || connection.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection must be open to execute queries.");
                }

                string query = @"
                    SELECT e.id, e.name, e.force, e.level, e.mechanic, e.equipment, e.category, e.image_url,
                           i.step_number, i.description,
                           m.id AS muscle_id, m.name AS muscle_name, em.type
                    FROM exercises e
                    LEFT JOIN instructions i ON e.id = i.exercise_id
                    LEFT JOIN exercisemuscles em ON e.id = em.exercise_id
                    LEFT JOIN muscles m ON em.muscle_id = m.id
                    WHERE e.id = @exerciseId
                    ORDER BY e.id, i.step_number, em.type";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@exerciseId", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (exercise == null)
                            {
                                exercise = new ExerciseDetail
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Force = reader.GetString(2),
                                    Level = reader.GetString(3),
                                    Mechanic = reader.GetString(4),
                                    Equipment = reader.GetString(5),
                                    Category = reader.GetString(6),
                                    ImageUrl = reader.IsDBNull(7) ? null : reader.GetString(7)
                                };
                            }

                            if (!reader.IsDBNull(8))
                            {
                                var instruction = new Instruction
                                {
                                    StepNumber = reader.GetInt32(8),
                                    Description = reader.GetString(9)
                                };
                                exerciseDetail.Instructions.Add(instruction);
                            }

                            if (!reader.IsDBNull(10))
                            {
                                var muscle = new Muscle
                                {
                                    Id = reader.GetInt32(10),
                                    Name = reader.GetString(11),
                                    Type = reader.GetString(12)
                                };

                                if (muscle.Type == "primary")
                                {
                                    exerciseDetail.PrimaryMuscles.Add(muscle);
                                }
                                else if (muscle.Type == "secondary")
                                {
                                    exerciseDetail.SecondaryMuscles.Add(muscle);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }
    }
}
