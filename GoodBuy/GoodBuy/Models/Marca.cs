using GoodBuy.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using GoodBuy.Service;

namespace GoodBuy.Models
{
    [DataTable(nameof(Marca))]
    public class Marca : BaseEntity
    {
        [JsonProperty("nome")]
        public string Nome { get; private set; }
        [JsonIgnore]
        public IEnumerable<Produto> Produtos { get; set; }

        public Marca(string nome)
        {
            Nome = nome;
        }
        public Marca()
        {

        }

        //public async override void LoadRelationalEntitiesCollections(AzureService entityService)
        //{
        //    var produtos = new List<Produto>();
        //    var idsProdutos = await entityService.GetTable<Many_to_Many.CarteiraProduto>().Where(x => x.IdMarca == Id).Select(x => x.IdProduto).ToListAsync();
        //    produtos.AddRange(await entityService.GetTable<Produto>().Where(x => idsProdutos.Contains(x.Id)).ToListAsync());
        //}
    }
}
