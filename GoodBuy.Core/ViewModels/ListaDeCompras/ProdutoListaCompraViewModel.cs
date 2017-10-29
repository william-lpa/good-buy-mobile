using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using Xamarin.Forms;
using GoodBuy.Log;
using System;
using System.Linq;

namespace GoodBuy.ViewModels.ListaDeCompras
{
    public class ProdutoListaCompraViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly ListaCompraService listaCompraService;
        private string produto;
        private string marca;
        private float quantidade;
        private string unidadeMedida;
        private bool isNotSearching;
        private bool marcaHasValue;
        private ListaDeComprasDetalhePageViewModel listaDeComprasDetalhePageViewModel;
        public bool IsNotSearching
        {
            get => isNotSearching;
            set => SetProperty(ref isNotSearching, value);
        }
        public bool MarcaHasValue
        {
            get => marcaHasValue;
            set => SetProperty(ref marcaHasValue, value);
        }

        public string Produto
        {
            get { return produto; }
            set
            {
                SetProperty(ref produto, value);
            }
        }
        public string Marca
        {
            get { return marca; }
            set
            {
                SetProperty(ref marca, value);
            }
        }
        public float Quantidade
        {
            get { return quantidade; }
            set
            {
                if (SetProperty(ref quantidade, value))
                {
                    if (ProdutoListaCompra != null)
                        ProdutoListaCompra.QuantidadeMensuravel = value;
                }
            }
        }
        public string UnidadeMedida
        {
            get { return unidadeMedida; }
            set
            {
                SetProperty(ref unidadeMedida, value);
            }
        }
        public string ProdutoTipoDescription => ProdutoListaCompra.Produto?.Nome + (ProdutoListaCompra.Produto?.Tipo != null ? " - " + ProdutoListaCompra.Produto?.Tipo?.Nome : "");

        public ProdutoListaCompra ProdutoListaCompra { get; set; }
        public Command VincularMarcaAoProdutoCommand { get; }
        public Command ExcluirProdutoListaCommand { get; }

        private ProdutoListaCompraViewModel(ListaDeComprasDetalhePageViewModel listaDeComprasDetalhePageViewModel)
        {
            this.listaDeComprasDetalhePageViewModel = listaDeComprasDetalhePageViewModel;
            VincularMarcaAoProdutoCommand = new Command(ExecuteVinculoProdutoMarcaAoPedido);
            ExcluirProdutoListaCommand = new Command(ExecuteExclusaoProdutoLista);
            IsNotSearching = listaDeComprasDetalhePageViewModel.IsNotSearching;
        }

        public ProdutoListaCompraViewModel(AzureService azure, ListaDeComprasDetalhePageViewModel listaDeComprasDetalhePageViewModel, ListaCompraService listaCompra,
                                           Produto produto)
                : this(listaDeComprasDetalhePageViewModel)
        {
            this.azureService = azure;
            this.listaCompraService = listaCompra;
            Produto = produto.Nome;
            ProdutoListaCompra = new ProdutoListaCompra(produto.Id) { Produto = produto };
        }

        public ProdutoListaCompraViewModel(AzureService azureService, ListaDeComprasDetalhePageViewModel listaDeComprasDetalhePageViewModel,
                                           ListaCompraService listaCompraService, ProdutoListaCompra produtoListaCompra)
                : this(listaDeComprasDetalhePageViewModel)
        {
            this.azureService = azureService;
            this.listaCompraService = listaCompraService;
            ProdutoListaCompra = produtoListaCompra;
            Adapter();
        }


        private void ExecuteExclusaoProdutoLista()
        {
            var produtoListaCompra = Transform();
            if (produtoListaCompra.Id != null)
                listaCompraService.ExcluirProdutoListaCompraAsync(produtoListaCompra);
            listaDeComprasDetalhePageViewModel.ProdutosListaCompra.First(x => x.Key == Produto[0]).Remove(this);
        }

        private void Adapter()
        {
            System.Threading.Tasks.Task.Delay(20).Wait();
            Produto = ProdutoListaCompra.Produto?.Nome;
            Marca = ProdutoListaCompra.Marca?.Nome;
            if (Marca != null)
                MarcaHasValue = true;
            UnidadeMedida = ProdutoListaCompra.UnidadeMedida?.Nome;
            Quantidade = ProdutoListaCompra.QuantidadeMensuravel;
            OnPropertyChanged(ProdutoTipoDescription);
        }
        public ProdutoListaCompra Transform()
        {
            if (ProdutoListaCompra.Produto != null)
                ProdutoListaCompra.Produto.Nome = Produto;
            if (Marca != null)
                ProdutoListaCompra.Marca = new Marca(Marca);
            if (UnidadeMedida != null)
                ProdutoListaCompra.UnidadeMedida = new UnidadeMedida(UnidadeMedida);  // chamar o service de ofertas pra criar ou retornar id
            ProdutoListaCompra.QuantidadeMensuravel = Quantidade;
            OnPropertyChanged(ProdutoTipoDescription);
            return ProdutoListaCompra;
        }

        private async void ExecuteVinculoProdutoMarcaAoPedido()
        {
            var marcas = await listaCompraService.ObterMarcasPorProduto(ProdutoListaCompra.Produto.Id);

            var result = await MessageDisplayer.Instance.ShowOptionsAsync($"Vincular marca a {Produto}", "Cancelar", null, marcas);
            if (result.ToLower() != "cancelar")
            {
                Marca = result;
                MarcaHasValue = true;
            }
        }
    }
}
