using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using GoodBuy.Models.Logical;
using GoodBuy.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class OfertasPageViewModel : BaseViewModel
    {
        private AzureService azureService;
        public ObservableCollection<OfertaDto> Ofertas { get; private set; }
        public OfertasPageViewModel(AzureService service)
        {
            this.azureService = service;
            InitializeOfertasAsync();
            Ofertas = new ObservableCollection<OfertaDto>();
        }

        private async void InitializeOfertasAsync()
        {
            var ofertas = await azureService.GetTable<Oferta>().ToListAsync();
            Dictionary<string, Estabelecimento> estabelecimentos = null;
            Dictionary<string, CarteiraProduto> produtoCarteira = null;
            Dictionary<string, Produto> produtos = null;
            Dictionary<string, Marca> marcas;

            Task.Factory.StartNew(
               async () => estabelecimentos = (await azureService.GetTable<Estabelecimento>()
               //.Where(x => ofertas.Select(oferta => oferta.IdEstabelecimento).Distinct().Contains(x.Id))
               .ToListAsync()).ToDictionary(key => key.Id, value => value)).Wait();

            var carteiraProdutoJob = Task.Factory.StartNew(
                async () => produtoCarteira = (await azureService.GetTable<CarteiraProduto>()
                //.Where(x => ofertas.Select(oferta => oferta.IdCarteiraProduto).Distinct().Contains(x.Id))
                .ToListAsync()).ToDictionary(key => key.Id, value => value))
                .ContinueWith(
                (pr) =>
                {
#pragma warning disable CS4014
                    Task.Factory.StartNew(async () => produtos = (await azureService.GetTable<Produto>()
                    //.Where(x => produtoCarteira.Values.Select(produto => produto.IdProduto).Distinct().Contains(x.Id))
                    .ToListAsync()).ToDictionary(key => key.Id, value => value), TaskCreationOptions.AttachedToParent);

                    Task.Factory.StartNew(async () => marcas = (await azureService.GetTable<Marca>()
                    //.Where(x => produtoCarteira.Values.Select(marca => marca.IdMarca).Distinct().Contains(x.Id))
                    .ToListAsync()).ToDictionary(key => key.Id, value => value), TaskCreationOptions.AttachedToParent);
#pragma warning restore CS4014

                });

            //estabelecimentosJob
            Task.WaitAll(new Task[] { carteiraProdutoJob });

            Ofertas = new ObservableCollection<OfertaDto>(
                ofertas.Select(x => new OfertaDto(estabelecimentos[x.IdEstabelecimento], x))
                );

        }
    }
}

