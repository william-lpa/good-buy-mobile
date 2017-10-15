using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Autofac;
using GoodBuy.Log;
using System.Collections.Generic;
using System.Linq;

namespace GoodBuy.ViewModels
{
    class NovoGrupoOfertaPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        private readonly UserService userService;
        public GenericRepository<User> UserRepository { get; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetProperty(ref name, value))
                    PersistGrupoOfertaCommand.ChangeCanExecute();
            }
        }
        public bool Private { get; set; }
        public bool ResetSelection { get; set; }

        public Command PersistGrupoOfertaCommand { get; }
        public Command SearchContact { get; }
        public Command RemoverParticipanteSelecionadoCommand { get; }
        public Command RemoverGrupoCommand { get; }
        public Command SearchUser { get; }
        public Command InteractGrupoOfertaCommand { get; }
        public Command UserSelectedCommand { get; }

        public ObservableCollection<ParticipanteGrupo> Members { get; }
        public bool EditingGroup { get; set; }
        public string PrimaryAction => EditingGroup ? "Salvar" : "Criar";

        public string SearchText { get; set; }

        public string SecondaryAction => UsuarioLogadoPertenceAoGrupo ? "Sair" : "Participar";
        public ObservableCollection<ParticipanteGrupo> CachedList { get; private set; }

        private GrupoOferta editGrupoOferta;
        private ParticipanteGrupo participanteGrupoOferta;

        public NovoGrupoOfertaPageViewModel(AzureService azureService, GrupoOfertaService service, UserService userService)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            this.userService = userService;
            UserRepository = new GenericRepository<User>(azureService);
            PersistGrupoOfertaCommand = new Command(SalvarGrupoUsuarioAsync, PodeCriarGrupoOferta);
            InteractGrupoOfertaCommand = new Command(ExecutePermanenciaGrupoAsync, PodeInteragirGrupoOferta);
            SearchContact = new Command(ExecuteOpenContactList);
            RemoverGrupoCommand = new Command(ExcluirGrupoOfertaAsync, PodeExcluirGrupo);
            RemoverParticipanteSelecionadoCommand = new Command(ExecuteRemoverParticipanteSelecionadoAsync, PodeExcluirParticipante);
            UserSelectedCommand = new Command<ParticipanteGrupo>(ExecuteStoreParticipanteAsync);
            SearchUser = new Command<string>(ExecuteSearchUserAsync);
            Members = new ObservableCollection<ParticipanteGrupo>();
            CachedList = new ObservableCollection<ParticipanteGrupo>();
        }


        private async void ExecuteStoreParticipanteAsync(ParticipanteGrupo participante)
        {
            if (CachedList.Count > 0)
            {//está pesquisando
                AdicionarParticipante(participante);
                await grupoOfertaService.ParticiparGrupoAsync(editGrupoOferta, Members.Last());
                CachedList.Add(participante);
                SearchText = string.Empty;
                //ExecuteSearchUser("");
                AtualizarStatus();
                return;
            }
            this.participanteGrupoOferta = participante;
            RemoverParticipanteSelecionadoCommand.ChangeCanExecute();
        }

        private async void ExecutePermanenciaGrupoAsync()
        {
            if (UsuarioLogadoPertenceAoGrupo)
            {
                if (await MessageDisplayer.Instance.ShowAskAsync("Deixar o grupo de oferta", $"Você tem certeza que deseja sair do grupo {editGrupoOferta.Name} ?", "Sim", "Não"))
                {
                    var membro = Members.First(x => x.IdUser == azureService.CurrentUser.User.Id);
                    Members.Remove(membro);
                    await grupoOfertaService.ExcluirParticipanteGrupoOfertaAsync(membro);
                    await PopAsync<GruposOfertasPageViewModel>();
                }
            }
            else
            {
                if (await MessageDisplayer.Instance.ShowAskAsync("Participar do grupo de oferta", $"Você tem certeza que deseja participar do grupo {editGrupoOferta.Name} ?", "Sim", "Não"))
                {
                    var user = azureService.CurrentUser.User;
                    AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
                    await grupoOfertaService.ParticiparGrupoAsync(editGrupoOferta, Members.Last());
                    AtualizarStatus();
                }
            }
        }

        private void AtualizarStatus()
        {
            PersistGrupoOfertaCommand.ChangeCanExecute();
            InteractGrupoOfertaCommand.ChangeCanExecute();
            RemoverGrupoCommand.ChangeCanExecute();
            RemoverParticipanteSelecionadoCommand.ChangeCanExecute();
            OnPropertyChanged(nameof(SecondaryAction));
            OnPropertyChanged(nameof(SearchText));
        }

        private async void ExcluirGrupoOfertaAsync()
        {
            if (await MessageDisplayer.Instance.ShowAskAsync("Excluir grupo de oferta", "Você tem certeza que deseja excluir o grupo?", "Sim", "Não"))
            {
                await grupoOfertaService.ExcluirGrupoOfertaAsync(editGrupoOferta);
                await PopAsync<GruposOfertasPageViewModel>();
            }
        }
        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                EditingGroup = true;
                editGrupoOferta = await grupoOfertaService.CarregarGrupoOfertaPorIdAsync(parameters["ID"]);
                AdapterAsync();
            }
            else
            {
                EditingGroup = false;
                var user = azureService.CurrentUser.User;
                AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user, Owner = true });
            }
        }

        private GrupoOferta TransformEditGrupoOferta()
        {

            editGrupoOferta.Name = Name;
            editGrupoOferta.Private = Private;
            editGrupoOferta.Participantes = Members;
            return editGrupoOferta;
        }

        private async void AdapterAsync()
        {
            Name = editGrupoOferta.Name;
            Private = editGrupoOferta.Private;
            editGrupoOferta.Participantes = await grupoOfertaService.CarregarParticipantesPorIdGrupoOfertaAsync(editGrupoOferta.Id);

            foreach (var participante in editGrupoOferta.Participantes)
            {
                participante.NomeGrupo = Name;
                AdicionarParticipante(participante);
            }
            AtualizarStatus();
        }

        private async void ExecuteSearchUserAsync(string expression)
        {
            if (expression == null)
                return;

            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                CachedList = new ObservableCollection<ParticipanteGrupo>(Members);
            }
            Members.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                foreach (var item in CachedList)
                {
                    Members.Add(item);
                }
                CachedList.Clear();
                return;
            }

            var users = await userService.LocalizarUsuariosPesquisadosAsync(expression);
            if (users != null)
                foreach (var user in users)
                {
                    AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
                }
        }


        private async void ExecuteRemoverParticipanteSelecionadoAsync()
        {
            if (Members.Count >= 1)
            {
                Members.Remove(participanteGrupoOferta);
                await grupoOfertaService.ExcluirParticipanteGrupoOfertaAsync(participanteGrupoOferta);
                participanteGrupoOferta = null;
                AtualizarStatus();
            }
        }

        private void ExecuteOpenContactList()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IContactListService>().PickContactList(ContactPickedAsync);
        }

        private async void ContactPickedAsync(User user)
        {
            if (await UserRepository.GetByIdAsync(user.Id) != null)
            {
                AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
            }
            else
                await MessageDisplayer.Instance.ShowMessageAsync("Usuário não encontrado", "O contato selecionado não está utilizando o aplicativo, portanto não pode ser incluso no grupo", "OK");
        }

        private void AdicionarParticipante(ParticipanteGrupo participante)
        {
            if (Members.FirstOrDefault(x => participante.IdUser == x.IdUser) == null)
            {
                participante.NomeGrupo = Name;
                Members.Add(participante);
            }
            PersistGrupoOfertaCommand.ChangeCanExecute();
        }

        public async void SalvarEdicaoAsync(bool syncronize = false)
        {
            if (editGrupoOferta.Name != Name || editGrupoOferta.Participantes.Count() != Members.Count || editGrupoOferta.Private != Private)
                await grupoOfertaService.AtualizarNovoGrupoUsuarioAsync(TransformEditGrupoOferta(), syncronize);
        }

        private async void SalvarGrupoUsuarioAsync()
        {
            if (EditingGroup)
            {
                SalvarEdicaoAsync();
            }
            else
            {
                var oferta = new GrupoOferta(Name, Private);
                await grupoOfertaService.CadastrarNovoGrupoUsuarioAsync(oferta, Members.ToList());
            }
            await PopAsync<GruposOfertasPageViewModel>();
        }

        public bool UsuarioLogadoPertenceAoGrupo => Members.Select(x => x.IdUser).Contains(azureService.CurrentUser.User.Id);

        private bool UsuarioLogadoCriouOGrupo => Members.FirstOrDefault(x => x.IdUser == azureService.CurrentUser.User.Id)?.Owner ?? false;

        private bool PodeInteragirGrupoOferta() => EditingGroup;

        private bool PodeExcluirParticipante() => PodeInteragirGrupoOferta() && UsuarioLogadoPertenceAoGrupo && participanteGrupoOferta != null && CachedList?.Count == 0;

        private bool PodeExcluirGrupo() => PodeInteragirGrupoOferta() && UsuarioLogadoCriouOGrupo;


        private bool PodeCriarGrupoOferta()
        {
            if (EditingGroup)
                return UsuarioLogadoPertenceAoGrupo;
            else
                return !string.IsNullOrWhiteSpace(Name) && Members.Count > 0; //alterar pra 1 dps
        }
    }
}
