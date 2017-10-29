using Autofac;
using GoodBuy.Log;
using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace GoodBuy.ViewModels.ListaDeCompras
{
    public class ListaDeComprasPageParticipantesViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly ListaCompraService listaCompraService;
        private readonly UserService userService;
        public GenericRepository<User> UserRepository { get; }
        public bool ResetSelection { get; set; }
        public Command SearchContact { get; }
        public Command RemoverParticipanteSelecionadoCommand { get; }
        public Command SearchUser { get; }
        public Command UserSelectedCommand { get; }
        public ObservableCollection<ParticipanteLista> Members { get; set; }
        public ObservableCollection<ParticipanteLista> CachedList { get; private set; }
        public string SearchText { get; set; }
        ListaDeComprasDetalhePageViewModel editListaCompraDetalheViewModel;
        private ParticipanteLista participanteListaCompra;

        public ListaDeComprasPageParticipantesViewModel(AzureService azureService, UserService userService, ListaCompraService listaCompraService,
                                                        ListaDeComprasDetalhePageViewModel listaCompraDetalheViewModel)
        {
            this.azureService = azureService;
            this.listaCompraService = listaCompraService;
            this.userService = userService;
            editListaCompraDetalheViewModel = listaCompraDetalheViewModel;
            UserRepository = new GenericRepository<User>(azureService);
            SearchContact = new Command(ExecuteOpenContactList);
            RemoverParticipanteSelecionadoCommand = new Command(ExecuteRemoverParticipanteSelecionadoAsync, PodeExcluirParticipante);
            UserSelectedCommand = new Command<ParticipanteLista>(ExecuteStoreParticipanteAsync);
            SearchUser = new Command<string>(ExecuteSearchUserAsync);
            CachedList = new ObservableCollection<ParticipanteLista>();
            Members = new ObservableCollection<ParticipanteLista>();
            Adapter();
        }

        private void ExecuteStoreParticipanteAsync(ParticipanteLista participante)
        {
            if (CachedList.Count > 0)
            {//está pesquisando
                Members.Add(editListaCompraDetalheViewModel.AdicionarParticipante(participante));
                CachedList.Add(participante);
                SearchText = string.Empty;
                AtualizarStatus();
                return;
            }
            this.participanteListaCompra = participante;
            RemoverParticipanteSelecionadoCommand.ChangeCanExecute();
        }
        private void AtualizarStatus()
        {
            RemoverParticipanteSelecionadoCommand.ChangeCanExecute();
            OnPropertyChanged(nameof(SearchText));
        }

        private ListaCompra TransformEditGrupoOferta()
        {
            editListaCompraDetalheViewModel.EditList.Participantes = Members;
            return editListaCompraDetalheViewModel.EditList;
        }

        private void Adapter()
        {
            foreach (var participante in editListaCompraDetalheViewModel.Members)
            {
                Members.Add(participante);
            }
            AtualizarStatus();
        }

        private async void ExecuteSearchUserAsync(string expression)
        {
            if (expression == null)
                return;

            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                CachedList = new ObservableCollection<ParticipanteLista>(Members);
            }
            Members.Clear();
            editListaCompraDetalheViewModel.Members.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                foreach (var item in CachedList)
                {
                    Members.Add(editListaCompraDetalheViewModel.AdicionarParticipante(item));
                }
                CachedList.Clear();
                return;
            }

            var users = await userService.LocalizarUsuariosPesquisadosAsync(expression);
            if (users != null)
                foreach (var user in users)
                {
                    Members.Add(editListaCompraDetalheViewModel?.AdicionarParticipante(new ParticipanteLista(user.Id) { User = user }));
                }
        }

        private async void ExecuteRemoverParticipanteSelecionadoAsync()
        {
            if (Members.Count >= 1)
            {
                editListaCompraDetalheViewModel.Members.Remove(participanteListaCompra);
                Members.Remove(participanteListaCompra);
                await listaCompraService.ExcluirParticipanteListaCompraAsync(participanteListaCompra);
                participanteListaCompra = null;
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
                editListaCompraDetalheViewModel?.AdicionarParticipante(new ParticipanteLista(user.Id) { User = user });
            }
            else
                await MessageDisplayer.Instance.ShowMessageAsync("Usuário não encontrado", "O contato selecionado não está utilizando o aplicativo, portanto não pode ser incluso no grupo", "OK");
        }

        public bool UsuarioLogadoPertenceAoGrupo => Members?.Select(x => x.IdUser)?.Contains(azureService.CurrentUser.User.Id) ?? false;

        private bool UsuarioLogadoCriouOGrupo => Members.FirstOrDefault(x => x.IdUser == azureService.CurrentUser.User.Id)?.Owner ?? false;

        private bool PodeExcluirParticipante() => UsuarioLogadoCriouOGrupo && participanteListaCompra != null && CachedList?.Count == 0;
    }
}
