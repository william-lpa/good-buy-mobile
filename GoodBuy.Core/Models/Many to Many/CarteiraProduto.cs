﻿using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

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
        public CarteiraProduto(string idProduto)
        {
            IdProduto = idProduto;
        }
        public CarteiraProduto()
        {

        }
    }
}
