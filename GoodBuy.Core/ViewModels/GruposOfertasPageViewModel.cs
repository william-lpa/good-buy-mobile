using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class GruposOfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        public Command NovoGrupoCommand { get; }
        public Command EditGroupCommand { get; }
        public ObservableCollection<GrupoOferta> GruposOfertasUsuario { get; private set; }

        public GruposOfertasPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            NovoGrupoCommand = new Command(ExecuteCriarNovoGrupoOferta);
            EditGroupCommand = new Command<GrupoOferta>(ExecuteEditarGroupoOfertas);
            GruposOfertasUsuario = new ObservableCollection<GrupoOferta>();
        }

        private async void ExecuteEditarGroupoOfertas(GrupoOferta grupoOferta)
        {
            var parameters = new Dictionary<string, string>();
            parameters.Add("ID", grupoOferta.Id);
            await PushAsync<NovoGrupoOfertaPageViewModel>(false, parameters);
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            GruposOfertasUsuario.Clear();
            var grupos = await grupoOfertaService.CarregarGrupoDeOfertasUsuarioLogado();
            foreach (var grupo in grupos)
            {
                GruposOfertasUsuario.Add(grupo);
            }
        }

        private async void ExecuteCriarNovoGrupoOferta()
        {
            await PushAsync<NovoGrupoOfertaPageViewModel>();
        }
    }
}
