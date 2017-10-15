using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using GoodBuy.Service;

namespace GoodBuy.Models
{
    [DataTable(nameof(Marca))]
    public class Marca : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonIgnore]
        public IEnumerable<Produto> Produtos { get; set; }

        public Marca(string nome)
        {
            Nome = nome;
        }
        public Marca()
        {

        }
    }
}
