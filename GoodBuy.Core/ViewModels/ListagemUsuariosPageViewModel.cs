using Autofac;
using GoodBuy.Core.Models.Logical;
using GoodBuy.Log;
using GoodBuy.Models;
using GoodBuy.Service;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class ListagemUsuariosPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
        private readonly UserService userService;

        public Command SearchUser { get; }
        public Command UserSelectedCommand { get; }
        public Command SearchContact { get; }
        public string SearchText { get; set; }
        public ObservableCollection<User> Users { get; }
        public GenericRepository<User> UserRepository { get; }

        public ListagemUsuariosPageViewModel(AzureService azureService, GrupoOfertaService service, UserService userService)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            this.userService = userService;
            SearchContact = new Command(ExecuteOpenContactList);
            UserSelectedCommand = new Command<User>(ExecuteCompartilharOFertaUsuarioAsync);
            SearchUser = new Command<string>(ExecuteSearchUserAsync);
            Users = new ObservableCollection<User>();
            Init();
        }
        protected override void Init(Dictionary<string, string> parameters = null)
        {
            ExecuteSearchUserAsync("");
        }

        private async void ExecuteCompartilharOFertaUsuarioAsync(User user)
        {
            if (await MessageDisplayer.Instance.ShowAskAsync("Compartilhar Oferta", $"Você tem certeza que deseja compartilhar a oferta selecionada com {user.FullName} ?", "Sim", "Não"))
            {
                var body = new CompartilhamentoOfertaUsuario()
                {
                    Title = $"Nova oferta Compartilhada",
                    Description = $"{azureService.CurrentUser.User.FullName} compartilhou uma nova oferta, clique para mais detalhes",
                    IdOferta = CompartilharOfertasPageViewModel.IdSharingOferta,
                    IdUser = user.Id,
                };

                await azureService.Client.InvokeApiAsync<CompartilhamentoOfertaUsuario, CompartilhamentoOfertaUsuario>("compartilharOfertaUsuario", body);
                await PopAsync<OfertasPageViewModel>();
            }

        }
        private async void ExecuteSearchUserAsync(string expression)
        {
            if (expression == null)
                return;

            if (expression.Length > 0 && expression.Length <= 2)
                return;

            var users = await userService.LocalizarUsuariosPesquisadosAsync(expression);
            Users.Clear();
            if (users != null)
                foreach (var user in users)
                    Users.Add(user);
        }

        private void ExecuteOpenContactList()
        {
            using (var scope = App.Container.BeginLifetimeScope())
                scope.Resolve<IContactListService>().PickContactList(ContactPickedAsync);
        }

        private async void ContactPickedAsync(User user)
        {
            if (await UserRepository.GetByIdAsync(user.Id) != null)
                ExecuteCompartilharOFertaUsuarioAsync(user);
            else
                await MessageDisplayer.Instance.ShowMessageAsync("Usuário não encontrado", "O contato selecionado não está utilizando o aplicativo, portanto não pode ser incluso no grupo", "OK");
        }
    }
}
