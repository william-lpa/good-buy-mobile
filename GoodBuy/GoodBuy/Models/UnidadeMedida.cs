using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(UnidadeMedida))]
    public class UnidadeMedida : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
