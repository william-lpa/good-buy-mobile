using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using GoodBuy.ViewModels.ListaDeCompras;

namespace GoodBuy.ViewModels
{
    public class ListaDeComprasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly ListaCompraService listaCompraService;
        public Command NovaListaCommand { get; }
        public Command EditListCommand { get; }
        public Command SearchList { get; }
        public ObservableCollection<ListaCompra> ListasUsuario { get; private set; }
        public ObservableCollection<ListaCompra> CachedList { get; set; }

        public ListaDeComprasPageViewModel(AzureService azureService, ListaCompraService service)
        {
            this.azureService = azureService;
            listaCompraService = service;
            NovaListaCommand = new Command(ExecuteCriarNovaListaCompraAsync);
            EditListCommand = new Command<ListaCompra>(ExecuteEditarGroupoOfertasAsync);
            SearchList = new Command<string>(ExecuteSearchList);
            ListasUsuario = new ObservableCollection<ListaCompra>();
            CachedList = new ObservableCollection<ListaCompra>();
        }

        private void ExecuteSearchList(string expression)
        {
            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                CachedList = new ObservableCollection<ListaCompra>(ListasUsuario);
            }
            ListasUsuario.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                foreach (var item in CachedList)
                {
                    ListasUsuario.Add(item);
                }
                CachedList.Clear();
                return;
            }
            foreach (var item in CachedList.Where(x => x.Nome.ToLower().Contains(expression.ToLower())).ToArray())
            {
                ListasUsuario.Add(item);
            }
        }

        private async void ExecuteEditarGroupoOfertasAsync(ListaCompra listaCompra)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", listaCompra.Id);
            await PushAsync<ListaDeComprasDetalhePageViewModel>(false, parameters);
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            ListasUsuario.Clear();
            var listas = await listaCompraService.CarregarListasDeComprasUsuarioLogadoAsync();
            foreach (var lista in listas)
            {
                ListasUsuario.Add(lista);
            }
            var novasListas = await listaCompraService.CarregarListasDeComprasUsuarioLogadoSync(listas.Select(x => x.Id));
            if (novasListas != null)
            {
                foreach (var lista in listas)
                {
                    ListasUsuario.Add(lista);
                }
            }
        }

        private async void ExecuteCriarNovaListaCompraAsync()
        {
            await PushAsync<ListaDeComprasDetalhePageViewModel>();
        }
    }
}
