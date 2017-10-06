using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoodBuy.Core.Models.Logical
{
    public class CompartilhamentoOfertaGrupo
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("idGrupo")]
        public string IdGrupo { get; set; }

        [JsonProperty("idOferta")]
        public string IdOferta { get; set; }
    }
}
