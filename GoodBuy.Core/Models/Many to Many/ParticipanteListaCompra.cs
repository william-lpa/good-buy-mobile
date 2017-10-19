using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(ParticipanteListaCompra))]
    public class ParticipanteListaCompra : BaseEntity
    {
        [JsonIgnore]
        public ListaCompra ListaCompra { get; set; }
        [JsonProperty("idListaCompra")]
        public string IdListaCompra{ get; set; }

        [JsonIgnore]
        public User User { get; set; }
        [JsonProperty("idUser")]
        public string IdUser { get; set; }

        //[JsonProperty("delete")]
        //public bool Delete { get; set; }
        
        public ParticipanteListaCompra()
        {

        }
    }
}
