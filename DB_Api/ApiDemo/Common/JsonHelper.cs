using Newtonsoft.Json;

namespace ApiDemo.Common
{
    public static class JsonHelper
    {
        public static Model? Deserialize<Model>(string jsonString)
        {
            return JsonConvert.DeserializeObject<Model>(jsonString);
        }

        public static string Serialize<Model>(Model model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
