using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class GruposOfertasPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        public Command NovoGrupoCommand { get; }
        public Command ListarParticipantesCommand { get; }
        public Command SairGrupoCommand { get; }
        public ObservableCollection<GrupoOferta> GruposOfertasUsuario { get; }

        public GruposOfertasPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            ListarParticipantesCommand = new Command(ExecuteExibirParticipantesGruposCommand);
            NovoGrupoCommand = new Command(ExecuteCriarNovoGrupoOferta);
            SairGrupoCommand = new Command(ExecuteSairNovoGrupoOferta);
            GruposOfertasUsuario = new ObservableCollection<GrupoOferta>(service.CarregarGrupoDeOfertasUsuarioLogado());
        }

        private void ExecuteSairNovoGrupoOferta()
        {

        }

        private void ExecuteCriarNovoGrupoOferta()
        {

        }

        private void ExecuteExibirParticipantesGruposCommand()
        {

        }
    }
}
