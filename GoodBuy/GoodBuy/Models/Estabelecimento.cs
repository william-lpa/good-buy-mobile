using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(Estabelecimento))]
    public class Estabelecimento : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonIgnore]
        public IEnumerable<Marca> Marcas { get; set; }

        [JsonIgnore]
        public IEnumerable<Produto> Produtos { get; set; }

        public Estabelecimento(string nome)
        {
            Nome = nome;
        }
    }
}
