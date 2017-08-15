using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(Sabor))]
    public class Sabor : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
