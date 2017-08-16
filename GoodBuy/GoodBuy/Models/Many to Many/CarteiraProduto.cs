using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodBuy.Models.Many_to_Many
{
    [DataTable(nameof(CarteiraProduto))]
    public class CarteiraProduto : BaseEntity
    {
        [JsonProperty("idProduto")]
        public string IdProduto { get; private set; }
        [JsonIgnore]
        public Produto Produto { get; set; }

        [JsonProperty("idMarca")]
        public string IdMarca { get; private set; }
        [JsonIgnore]
        public Marca Marca { get; set; }

        public CarteiraProduto(string idProduto, string idMarca)
        {
            IdProduto = idProduto;
            IdMarca = idMarca;
        }
        public CarteiraProduto()
        {

        }
    }
}
