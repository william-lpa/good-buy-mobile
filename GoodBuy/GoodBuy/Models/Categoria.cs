using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(Categoria))]
    public class Categoria : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonIgnore]
        public IEnumerable<Produto> Produtos { get; set; }

    }
}
