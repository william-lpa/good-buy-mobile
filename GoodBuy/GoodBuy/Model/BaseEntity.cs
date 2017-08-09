using Newtonsoft.Json;

namespace GoodBuy.Model
{
    public class BaseEntity:IEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}