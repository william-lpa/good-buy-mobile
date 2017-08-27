using GoodBuy.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Models
{
    public class UserFriends:BaseEntity
    {
        [JsonProperty("idUsuario1")]
        public long IdUser1{ get; set; }
        [JsonIgnore]
        public User Usuario1 { get; set; }

        [JsonProperty("idUsuario2")]
        public long IdUser2 { get; set; }
        [JsonIgnore]
        public User Usuario2 { get; set; }

    }
}
