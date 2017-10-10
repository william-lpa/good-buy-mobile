using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(Categoria))]
    public class Categoria : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonIgnore]
        public IEnumerable<Produto> Produtos { get; set; }

        public Categoria(string nome)
        {
            Nome = nome;
        }
        public Categoria()
        {

        }

    }
}
