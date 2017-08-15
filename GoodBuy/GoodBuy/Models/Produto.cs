using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(Produto))]
    public class Produto : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("idSabor")]
        public string IdSabor { get; set; }
        [JsonIgnore]
        public Sabor Sabor { get; set; }

        [JsonProperty("idCategoria")]
        public string IdCategoria { get; set; }
        [JsonIgnore]
        public Categoria Categoria { get; set; }

        [JsonProperty("idUnidadeMedida")]
        public string IdUnidadeMedida { get; set; }
        [JsonIgnore]
        public UnidadeMedida UnidadeMedida { get; set; }

        [JsonProperty("quantidadeMensuravel")]
        public float QuantidadeMensuravel { get; set; }

    }
}
