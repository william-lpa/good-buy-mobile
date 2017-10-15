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
    public class GruposOfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        public Command NovoGrupoCommand { get; }
        public Command EditGroupCommand { get; }
        public Command SearchPublicGroup { get; }
        public ObservableCollection<GrupoOferta> GruposOfertasUsuario { get; private set; }
        public ObservableCollection<GrupoOferta> CachedList { get; set; }
        private bool notShareMode;
        public bool NotSharing
        {
            get { return notShareMode; }
            set
            {
                SetProperty(ref notShareMode, value);
                if (!value) Init();
            }
        }

        public GruposOfertasPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            NotSharing = true;
            this.azureService = azureService;
            grupoOfertaService = service;
            NovoGrupoCommand = new Command(ExecuteCriarNovoGrupoOfertaAsync);
            EditGroupCommand = new Command<GrupoOferta>(ExecuteEditarGroupoOfertasAsync);
            SearchPublicGroup = new Command<string>(ExecuteSearchPublicGroupAsync);
            GruposOfertasUsuario = new ObservableCollection<GrupoOferta>();
            CachedList = new ObservableCollection<GrupoOferta>();
        }

        private async void ExecuteSearchPublicGroupAsync(string expression)
        {
            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                CachedList = new ObservableCollection<GrupoOferta>(GruposOfertasUsuario);
            }
            GruposOfertasUsuario.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                foreach (var item in CachedList)
                {
                    GruposOfertasUsuario.Add(item);
                }
                CachedList.Clear();
                return;
            }

            var result = await grupoOfertaService.LocalizarGruposOfertaPublicosAsync(expression);
            foreach (var item in result)
            {
                GruposOfertasUsuario.Add(item);
            }
        }

        private async void ExecuteEditarGroupoOfertasAsync(GrupoOferta grupoOferta)
        {
            if (NotSharing)
            {
                var parameters = new Dictionary<string, string>();
                parameters.Add("ID", grupoOferta.Id);
                await PushAsync<NovoGrupoOfertaPageViewModel>(false, parameters);
            }
            else
            {
                if (await MessageDisplayer.Instance.ShowAskAsync("Compartilhar Oferta", $"Você tem certeza que deseja compartilhar a oferta selecionada com o grupo {grupoOferta.Name} ?", "Sim", "Não"))
                {
                    var body = new CompartilhamentoOfertaGrupo()
                    {
                        Title = $"Nova oferta Compartilhada no grupo {grupoOferta.Name}",
                        Description = $"{azureService.CurrentUser.User.FullName} compartilhou uma nova oferta, clique para mais detalhes",
                        IdGrupo = grupoOferta.Id,
                        IdOferta = CompartilharOfertasPageViewModel.IdSharingOferta,
                    };

                    await azureService.Client.InvokeApiAsync<CompartilhamentoOfertaGrupo, CompartilhamentoOfertaGrupo>("compartilharOfertaGrupo", body);
                }
                await PopAsync<OfertasPageViewModel>();
            }
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            GruposOfertasUsuario.Clear();
            var grupos = await grupoOfertaService.CarregarGrupoDeOfertasUsuarioLogadoAsync();
            foreach (var grupo in grupos)
            {
                GruposOfertasUsuario.Add(grupo);
            }
            var newGroups = await grupoOfertaService.CarregarGrupoDeOfertasUsuarioLogadoSync(grupos.Select(x => x.Id));
            if (newGroups != null)
            {
                foreach (var grupo in grupos)
                {
                    GruposOfertasUsuario.Add(grupo);
                }
            }
        }

        private async void ExecuteCriarNovoGrupoOfertaAsync()
        {
            await PushAsync<NovoGrupoOfertaPageViewModel>();
        }
    }
}
