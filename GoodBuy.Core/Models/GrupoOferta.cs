using GoodBuy.Model;
using GoodBuy.Models.Many_to_Many;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodBuy.Models
{
    [DataTable(nameof(GrupoOferta))]
    public class GrupoOferta : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("private")]
        public bool Private { get; set; }
        [JsonIgnore]
        public IEnumerable<ParticipanteGrupo> Participantes { get; set; }

        [JsonIgnore]
        public string Privacity => Private ? "Grupo Privado" : "Grupo Público";
        [JsonIgnore]
        public string PrivacityImageGroup => Private ? "lock.png" : "unlock.png";

        public GrupoOferta(string name, bool restrict)
        {
            Name = name;
            Private = restrict;
        }
        public GrupoOferta()
        {

        }

    }
}
