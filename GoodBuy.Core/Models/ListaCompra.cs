using GoodBuy.Model;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(ListaCompra))]
    public class ListaCompra : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }

        [JsonIgnore]
        public IEnumerable<ParticipanteLista> Participantes { get; set; } = new List<ParticipanteLista>();
        [JsonIgnore]
        public IEnumerable<ProdutoListaCompra> ProdutosListaCompra { get; set; } = new List<ProdutoListaCompra>();

        public ListaCompra(string nome)
        {
            Nome = nome;
        }
        public ListaCompra() { }
    }
}
