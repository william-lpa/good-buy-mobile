using GoodBuy.Model;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(HistoricoOferta))]
    public class HistoricoOferta : BaseEntity
    {
        [JsonProperty("idOferta")]
        public long IdOferta { get; set; }
        [JsonIgnore]
        public Oferta Oferta { get; set; }

        [JsonProperty("preco")]
        public decimal Preco { get; set; }
    }
}

