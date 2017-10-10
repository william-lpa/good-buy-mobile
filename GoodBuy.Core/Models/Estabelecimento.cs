using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(Estabelecimento))]
    public class Estabelecimento : BaseEntity, IName
    {
        //[Version]
        //public string Version { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        //[JsonIgnore]
        //public IEnumerable<Marca> Marcas { get; set; }

        //[JsonIgnore]
        //public IEnumerable<Produto> Produtos { get; set; }

        public Estabelecimento(string nome)
        {
            Nome = nome;
        }

        public Estabelecimento()
        {

        }
    }
}
