using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace GoodBuy.Models
{
    [DataTable(nameof(User))]
    public class User: BaseEntity
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }
        [JsonProperty("facebookId")]
        public string FacebookId{ get; set; }
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("bithday")]
        public DateTime Bithday { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("male")]
        public bool Male { get; set; }

        
    }
}
