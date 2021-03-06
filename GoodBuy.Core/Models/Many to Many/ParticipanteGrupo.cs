﻿using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(ParticipanteGrupo))]
    public class ParticipanteGrupo : BaseEntity
    {
        [JsonIgnore]
        public GrupoOferta GrupoOferta { get; set; }
        [JsonProperty("idGrupoOferta")]
        public string IdGrupoOferta { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonProperty("idUser")]
        public string IdUser { get; set; }
        [JsonProperty("nomeGrupo")]
        public string NomeGrupo { get; set; }
        [JsonProperty("delete")]
        public bool Delete { get; set; }

        [JsonProperty("owner")]
        public bool Owner { get; set; }

        public ParticipanteGrupo(string idUser)
        {
            IdUser = idUser;
        }
        public ParticipanteGrupo()
        {

        }
    }
}
