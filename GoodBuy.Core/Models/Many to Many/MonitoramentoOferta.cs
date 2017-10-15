using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(MonitoramentoOferta))]
    public class MonitoramentoOferta : BaseEntity
    {
        public MonitoramentoOferta(string idOferta, string idUser)
        {
            IdOferta = idOferta;
            IdUser = idUser;
        }
        public MonitoramentoOferta()
        {

        }

        [JsonProperty("precoAlvo")]
        public decimal PrecoAlvo { get; set; }

        [JsonProperty("idUser")]
        public string IdUser { get; set; }

        [JsonProperty("idOferta")]
        public string IdOferta { get; set; }

        [JsonProperty("delete")]
        public bool Delete { get; set; }
    }
}
