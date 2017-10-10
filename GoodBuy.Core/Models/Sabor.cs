using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(Sabor))]
    public class Sabor : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        public Sabor(string nome)
        {
            Nome = nome;
        }
        public Sabor()
        {

        }
    }
}
