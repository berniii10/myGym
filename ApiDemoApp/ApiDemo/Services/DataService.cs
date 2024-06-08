using ApiDemo.Models;
using Newtonsoft.Json;

namespace ApiDemo.Services
{
    public class DataService
    {
        public DataService() { }

        private readonly string json_data_path = "data.json";

        public bool loadData(ref Model? model)
        {
            // Read JSON data from file
            string jsonData = File.ReadAllText(json_data_path);

            // Deserialize JSON data into a list of Model objects
            model = JsonConvert.DeserializeObject<Model>(jsonData);

            if (model == null)
            {
                return false;
            }else
            {
                return true;
            }
        }
    }
}
