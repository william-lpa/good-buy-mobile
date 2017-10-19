using GoodBuy.Model;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;

namespace GoodBuy.Models
{
    [DataTable(nameof(HistoricoOferta))]
    public class HistoricoOferta : BaseEntity
    {
       [JsonProperty("idOferta")]
        public string IdOferta { get; set; }
        [JsonIgnore]
        public Oferta Oferta { get; set; }

        [JsonProperty("preco")]
        public decimal Preco { get; set; }

        public HistoricoOferta(Oferta ofertaExistente)
        {
            IdOferta = ofertaExistente.Id;
            Oferta = ofertaExistente;
            Preco = ofertaExistente.PrecoAtual;
            //DataOferta = Pegar a dataCriacao do azure da oferta
        }
        public HistoricoOferta()
        {

        }

    }
}

