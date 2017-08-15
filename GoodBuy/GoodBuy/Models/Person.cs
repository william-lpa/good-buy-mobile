using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Model
{
    [DataTable("Person")]
    public class Person : BaseEntity
    {
        //[Version]
        //public string Version { get; set; } // don't change this, is important for Azure

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("male")]
        public bool Male { get; set; }
    }
}
