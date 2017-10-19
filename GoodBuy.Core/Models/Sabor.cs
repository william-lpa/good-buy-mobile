using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(Tipo))]
    public class Tipo : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        public Tipo(string nome)
        {
            Nome = nome;
        }
        public Tipo()
        {

        }
    }
}
