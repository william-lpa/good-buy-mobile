using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System;
using Autofac;
using GoodBuy.Log;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices;

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
            PersistGrupoOfertaCommand = new Command(SalvarGrupoUsuario, PodeCriarGrupoOferta);
            InteractGrupoOfertaCommand = new Command(ExecutePermanenciaGrupo, PodeInteragirGrupoOferta);
            SearchContact = new Command(ExecuteOpenContactList);
            RemoverGrupoCommand = new Command(ExcluirGrupoOferta, PodeCriarGrupoOferta);
            RemoverParticipanteSelecionadoCommand = new Command(ExecuteRemoverParticipanteSelecionado, PodeExcluirParticipante);
            UserSelectedCommand = new Command<ParticipanteGrupo>(ExecuteStoreParticipante);
            SearchUser = new Command<string>(ExecuteSearchUser);
            Members = new ObservableCollection<ParticipanteGrupo>();
            CachedList = new ObservableCollection<ParticipanteGrupo>();
        }


        private void ExecuteStoreParticipante(ParticipanteGrupo participante)
        {
            this.participanteGrupoOferta = participante;
            RemoverParticipanteSelecionadoCommand.ChangeCanExecute();
        }

        private async void ExecutePermanenciaGrupo()
        {
            if (UsuarioLogadoPertenceAoGrupo)
            {
                if (await MessageDisplayer.Instance.ShowAsk("Deixar o grupo de oferta", $"Você tem certeza que deseja sair do grupo {editGrupoOferta.Name} ?", "Sim", "Não"))
                {
                    var membro = Members.First(x => x.IdUser == azureService.CurrentUser.User.Id);
                    Members.Remove(membro);
                    await grupoOfertaService.ExcluirParticipanteGrupoOferta(membro);
                    AtualizarStatus();
                }
            }
            else
            {
                if (await MessageDisplayer.Instance.ShowAsk("Participar do grupo de oferta", $"Você tem certeza que deseja participar do grupo {editGrupoOferta.Name} ?", "Sim", "Não"))
                {
                    var user = azureService.CurrentUser.User;
                    AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
                    SalvarEdicao();
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
        }

        private async void ExcluirGrupoOferta()
        {
            if (await MessageDisplayer.Instance.ShowAsk("Excluir grupo de oferta", "Você tem certeza que deseja excluir o grupo?", "Sim", "Não"))
            {
                await grupoOfertaService.ExcluirGrupoOferta(editGrupoOferta);
                await PopAsync<GruposOfertasPageViewModel>();
            }
        }
        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                EditingGroup = true;
                editGrupoOferta = await grupoOfertaService.CarregarGrupoOfertaPorId(parameters["ID"]);
                Adapter();
            }
            else
            {
                EditingGroup = false;
                var user = azureService.CurrentUser.User;
                AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
            }
        }

        private GrupoOferta TransformEditGrupoOferta()
        {

            editGrupoOferta.Name = Name;
            editGrupoOferta.Private = Private;
            editGrupoOferta.Participantes = Members;
            return editGrupoOferta;
        }

        private async void Adapter()
        {
            Name = editGrupoOferta.Name;
            Private = editGrupoOferta.Private;
            editGrupoOferta.Participantes = await grupoOfertaService.CarregarParticipantesPorIdGrupoOferta(editGrupoOferta.Id);

            foreach (var participante in editGrupoOferta.Participantes)
            {
                participante.NomeGrupo = Name;
                AdicionarParticipante(participante);
            }
            AtualizarStatus();
        }

        private async void ExecuteSearchUser(string expression)
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

            var users = await userService.LocalizarUsuariosPesquisados(expression);
            if (users != null)
                foreach (var user in users)
                {
                    AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
                }
        }


        private async void ExecuteRemoverParticipanteSelecionado()
        {
            if (Members.Count >= 1)
            {
                Members.Remove(participanteGrupoOferta);
                await grupoOfertaService.ExcluirParticipanteGrupoOferta(participanteGrupoOferta);
                participanteGrupoOferta = null;
                AtualizarStatus();
            }
        }

        private void ExecuteOpenContactList()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IContactListService>().PickContactList(ContactPicked);
        }

        private async void ContactPicked(User user)
        {
            if (await UserRepository.GetById(user.Id) != null)
            {
                AdicionarParticipante(new ParticipanteGrupo(user.Id) { User = user });
            }
            else
                await MessageDisplayer.Instance.ShowMessage("Usuário não encontrado", "O contato selecionado não está utilizando o aplicativo, portanto não pode ser incluso no grupo", "OK");
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

        public async void SalvarEdicao()
        {
            if (editGrupoOferta.Name != Name || editGrupoOferta.Participantes.Count() != Members.Count || editGrupoOferta.Private != Private)
                await grupoOfertaService.AtualizarNovoGrupoUsuario(TransformEditGrupoOferta());
        }

        private async void SalvarGrupoUsuario()
        {
            if (EditingGroup)
            {
                SalvarEdicao();
            }
            else
            {
                var oferta = new GrupoOferta(Name, Private);
                await grupoOfertaService.CadastrarNovoGrupoUsuario(oferta, Members.ToList());
            }
            await PopAsync<GruposOfertasPageViewModel>();
        }

        public bool UsuarioLogadoPertenceAoGrupo => Members.Select(x => x.IdUser).Contains(azureService.CurrentUser.User.Id);

        private bool PodeInteragirGrupoOferta() => EditingGroup;

        private bool PodeExcluirParticipante() => PodeInteragirGrupoOferta() && UsuarioLogadoPertenceAoGrupo && participanteGrupoOferta != null && CachedList?.Count == 0;

        private bool PodeCriarGrupoOferta()
        {
            if (EditingGroup)
                return UsuarioLogadoPertenceAoGrupo;
            else
                return !string.IsNullOrWhiteSpace(Name) && Members.Count > 0; //alterar pra 1 dps
        }
    }
}
