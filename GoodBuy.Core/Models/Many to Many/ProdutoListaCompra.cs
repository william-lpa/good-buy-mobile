using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(ProdutoListaCompra))]
    public class ProdutoListaCompra : BaseEntity
    {
        public ProdutoListaCompra(string idProduto)
        {
            IdProduto = idProduto;
        }
        public ProdutoListaCompra() { }

        [JsonIgnore]
        public Produto Produto { get; set; }
        [JsonProperty("idProduto")]
        public string IdProduto { get; set; }

        [JsonIgnore]
        public Marca Marca { get; set; }
        [JsonProperty("idMarca")]
        public string IdMarca { get; set; }

        [JsonIgnore]
        public ListaCompra ListaCompra { get; set; }
        [JsonProperty("idListaCompra")]
        public string IdListaCompra { get; set; }


        [JsonProperty("quantidadeMensuravel")]
        public float QuantidadeMensuravel { get; set; }

        [JsonProperty("idUnidadeMedida")]
        public string IdUnidadeMedida { get; set; }

        [JsonIgnore]
        public UnidadeMedida UnidadeMedida { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; internal set; }
    }
}
