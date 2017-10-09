﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoodBuy.Core.Models.Logical
{
    public class CompartilhamentoOfertaUsuario
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("idUser")]
        public string IdUser { get; set; }

        [JsonProperty("idOferta")]
        public string IdOferta { get; set; }
    }
}
