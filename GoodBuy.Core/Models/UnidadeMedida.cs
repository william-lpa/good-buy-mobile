using GoodBuy.Model;
using GoodBuy.Service;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models
{
    [DataTable(nameof(UnidadeMedida))]
    public class UnidadeMedida : BaseEntity, IName
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        public UnidadeMedida(string nome)
        {
            Nome = nome;
        }

        public UnidadeMedida()
        {

        }
    }
}
