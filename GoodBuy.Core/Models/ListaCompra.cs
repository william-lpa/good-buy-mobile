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
        [JsonProperty("name")]
        public string Nome { get; set; }

        [JsonIgnore]
        public IEnumerable<ParticipanteListaCompra> Participantes { get; set; }
    }
}
