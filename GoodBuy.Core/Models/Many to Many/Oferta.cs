using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(Oferta))]
    public class Oferta : BaseEntity
    {
        [JsonProperty("idEstabelecimento")]
        public string IdEstabelecimento { get; private set; }
        [JsonIgnore]
        public Estabelecimento Estabelecimento { get; set; }

        [JsonProperty("idCarteiraProduto")]
        public string IdCarteiraProduto { get; private set; }
        [JsonIgnore]
        public CarteiraProduto CarteiraProduto { get; set; }

        [JsonProperty("precoAtual")]
        public decimal PrecoAtual { get; set; }
        //[JsonProperty("dataAtualizacao")]
        //public DateTime DataAtualizacao { get; set; }
        [JsonProperty("confiabilidade")]
        public float Confiabilidade { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public IEnumerable<HistoricoOferta> OfertasAnteriores { get; set; }

        public Oferta(string idEstabelecimento, string idCarteira, decimal preco)
        {
            IdEstabelecimento = idEstabelecimento;
            IdCarteiraProduto = idCarteira;
            PrecoAtual = preco;
        }
        public Oferta()
        {

        }

    }
}
