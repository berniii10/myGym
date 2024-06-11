using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiDemo.Models
{
    public class Actor
    {
        public string name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public int? age { get; set; } = int.MaxValue;
        public string birthday { get; set; } = string.Empty;
    }
}
