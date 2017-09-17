using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    class NovoGrupoOfertaPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        public GenericRepository<GrupoOferta> GrupoOfertaRepository { get; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetProperty(ref name, value))
                    CriarGrupoOfertaCommand.ChangeCanExecute();
            }
        }
        public bool Private { get; set; }
        public Command CriarGrupoOfertaCommand { get; }
        public ObservableCollection<ParticipanteGrupo> Members { get; }

        public NovoGrupoOfertaPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            GrupoOfertaRepository = new GenericRepository<GrupoOferta>(azureService);
            CriarGrupoOfertaCommand = new Command(CriarGrupoUsuario, PodeCriarGrupoOferta);
            Members = new ObservableCollection<ParticipanteGrupo>();
        }

        private void CriarGrupoUsuario()
        {
            var oferta = new GrupoOferta(Name, Private);
            //oferta.Participantes passar a lista de participantes.
        }

        private bool PodeCriarGrupoOferta() => !string.IsNullOrWhiteSpace(Name) && Members.Count > 2;
    }
}
