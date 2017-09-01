using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace GoodBuy.Models
{
    [DataTable(nameof(User))]
    public class User : BaseEntity
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }        
        [JsonProperty("avatar")]
        public string Avatar { get; set; }
        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("male")]
        public bool Male { get; set; }
        [JsonProperty("locale")]
        public string Locale { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
    }
}
