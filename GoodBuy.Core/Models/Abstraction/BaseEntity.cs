using Newtonsoft.Json;
using GoodBuy.Service.Interfaces;
using GoodBuy.Service;
using System;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodBuy.Model
{
    public abstract class BaseEntity : IEntity
    {
        [Version]
        public string Version { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

    }
}