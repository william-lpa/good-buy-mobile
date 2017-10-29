using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(ParticipanteLista))]
    public class ParticipanteLista : BaseEntity
    {
        [JsonIgnore]
        public ListaCompra ListaCompra { get; set; }

        [JsonProperty("idListaCompra")]
        public string IdListaCompra { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonProperty("idUser")]
        public string IdUser { get; set; }

        [JsonProperty("owner")]
        public bool Owner { get; set; }

        [JsonProperty("nomeLista")]
        public string NomeLista { get; internal set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }

        public ParticipanteLista()
        {

        }

        public ParticipanteLista(string idUser)
        {
            IdUser = idUser;
        }
    }
}
