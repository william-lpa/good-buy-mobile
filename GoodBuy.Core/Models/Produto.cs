using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(Produto))]
    public class Produto : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("idSabor")]
        public string IdSabor { get; private set; }
        [JsonIgnore]
        public Sabor Sabor { get; set; }

        [JsonProperty("idCategoria")]
        public string IdCategoria { get; private set; }
        [JsonIgnore]
        public Categoria Categoria { get; set; }

        [JsonProperty("idUnidadeMedida")]
        public string IdUnidadeMedida { get; private set; }
        [JsonIgnore]
        public UnidadeMedida UnidadeMedida { get; set; }

        [JsonProperty("quantidadeMensuravel")]
        public float QuantidadeMensuravel { get;  set; }

        public Produto(string nome, string idSabor, string idUnidadeMedida, string idCategoria, float quantidadeMensuravel)
        {
            Nome = nome;
            IdSabor = idSabor;
            IdUnidadeMedida = idUnidadeMedida;
            IdCategoria = idCategoria;
            QuantidadeMensuravel = quantidadeMensuravel;
        }
        public Produto()
        {

        }

    }
}
