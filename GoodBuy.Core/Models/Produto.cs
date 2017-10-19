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

        [JsonProperty("idTipo")]
        public string IdTipo { get; private set; }
        [JsonIgnore]
        public Tipo Tipo { get; set; }

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

        public Produto(string nome, string idTipo, string idUnidadeMedida, string idCategoria, float quantidadeMensuravel)
        {
            Nome = nome;
            IdTipo = idTipo;
            IdUnidadeMedida = idUnidadeMedida;
            IdCategoria = idCategoria;
            QuantidadeMensuravel = quantidadeMensuravel;
        }
        public Produto()
        {

        }

    }
}
