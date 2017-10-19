using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using GoodBuy.Log;
using GoodBuy.Core.Models.Logical;

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
            //grupoOfertaService = service;
            NovaListaCommand = new Command(ExecuteCriarNovoGrupoOfertaAsync);
            EditListCommand = new Command<ListaCompra>(ExecuteEditarGroupoOfertasAsync);
            SearchList = new Command<string>(ExecuteSearchList);
            ListasUsuario = new ObservableCollection<ListaCompra>();
            CachedList = new ObservableCollection<ListaCompra>();
        }

        private async void ExecuteSearchList(string expression)
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

            //var result = await listaCompraService.loca (expression);
            //foreach (var item in result)
            //{
            //    ListasUsuario.Add(item);
            //}
        }

        private async void ExecuteEditarGroupoOfertasAsync(ListaCompra listaCompra)
        {
                var parameters = new Dictionary<string, string>();
                parameters.Add("ID", listaCompra.Id);
                await PushAsync<NovoGrupoOfertaPageViewModel>(false, parameters);
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            ListasUsuario.Clear();
            var grupos = await listaCompraService.CarregarGrupoDeOfertasUsuarioLogadoAsync();
            foreach (var grupo in grupos)
            {
                ListasUsuario.Add(grupo);
            }
            var newGroups = await listaCompraService.CarregarGrupoDeOfertasUsuarioLogadoSync(grupos.Select(x => x.Id));
            if (newGroups != null)
            {
                foreach (var grupo in grupos)
                {
                    ListasUsuario.Add(grupo);
                }
            }
        }

        private async void ExecuteCriarNovoGrupoOfertaAsync()
        {
            await PushAsync<NovoGrupoOfertaPageViewModel>();
        }
    }
}
