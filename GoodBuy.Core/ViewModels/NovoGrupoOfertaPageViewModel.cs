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

namespace GoodBuy.ViewModels
{
    class NovoGrupoOfertaPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly GrupoOfertaService grupoOfertaService;
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
        public Command RemoverUltimoParticipanteCommand { get; }
        public Command RemoverGrupoCommand { get; }
        public ObservableCollection<ParticipanteGrupo> Members { get; }
        public bool EditingGroup { get; set; }
        public string PrimaryAction => EditingGroup ? "Salvar" : "Criar";
        private GrupoOferta editGrupoOferta;

        public NovoGrupoOfertaPageViewModel(AzureService azureService, GrupoOfertaService service)
        {
            this.azureService = azureService;
            grupoOfertaService = service;
            UserRepository = new GenericRepository<User>(azureService);
            PersistGrupoOfertaCommand = new Command(SalvarGrupoUsuario, PodeCriarGrupoOferta);
            SearchContact = new Command(ExecuteOpenContactList);
            RemoverGrupoCommand = new Command(ExcluirGrupoOferta);
            RemoverUltimoParticipanteCommand = new Command(ExecuteRemoverUltimoParticipante);
            Members = new ObservableCollection<ParticipanteGrupo>();
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
            IEnumerable<ParticipanteGrupo> participantes = await grupoOfertaService.CarregarParticipantesPorIdGrupoOferta(editGrupoOferta.Id);

            foreach (var participante in participantes)
            {
                AdicionarParticipante(participante);
            }
        }

        private void ExecuteRemoverUltimoParticipante()
        {
            if (Members.Count > 1)
                this.Members.RemoveAt(Members.Count - 1);
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
                Members.Add(participante);
            PersistGrupoOfertaCommand.ChangeCanExecute();
        }

        private async void SalvarGrupoUsuario()
        {
            if (EditingGroup)
            {
                await grupoOfertaService.AtualizarNovoGrupoUsuario(TransformEditGrupoOferta());
            }
            else
            {
                var oferta = new GrupoOferta(Name, Private);
                oferta.Participantes = Members;
                await grupoOfertaService.CadastrarNovoGrupoUsuario(oferta);
            }
            await PopAsync<GruposOfertasPageViewModel>();
        }

        private bool PodeCriarGrupoOferta() => !string.IsNullOrWhiteSpace(Name) && Members.Count > 0; //alterar pra 1 dps

    }
}
