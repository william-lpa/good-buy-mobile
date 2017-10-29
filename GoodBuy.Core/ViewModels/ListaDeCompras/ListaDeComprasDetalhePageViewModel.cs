using GoodBuy.Log;
using GoodBuy.Models;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using System;
using GoodBuy.Core.Controls;
using System.Threading.Tasks;
using Autofac;

namespace GoodBuy.ViewModels.ListaDeCompras
{
    public class ListaDeComprasDetalhePageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;
        private readonly ListaCompraService listaCompraService;

        public Command PersistListaCompra { get; }
        public Command SimularComprasCommand { get; }
        public Command SairListaComprasCommand { get; }
        public Command ListarParticipantesListaCommand { get; }
        public Command ProdutoListaCompraTappedCommand { get; }
        public Command SearchProduct { get; }
        public bool ResetSelection { get; set; }
        public ObservableCollection<ParticipanteLista> Members { get; set; }
        public ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>> CachedList { get; set; }
        public ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>> ProdutosListaCompra { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetProperty(ref name, value))
                    PersistListaCompra.ChangeCanExecute();
            }
        }
        public bool EditingList { get; set; }
        public bool IsNotSearching { get; set; } = true;
        public string SearchText { get; set; }
        public string PrimaryAction => EditingList ? "Salvar" : "Criar";
        public ListaCompra EditList { get; set; }

        public void GroupCollection(IEnumerable<Produto> produtos, IEnumerable<ProdutoListaCompra> produtoListaCompra = null)
        {
            ProdutoListaCompraViewModel[] produtosListaCompraViewModel = null;

            if (produtoListaCompra == null)
                produtosListaCompraViewModel = produtos.Select(x => new ProdutoListaCompraViewModel(azureService, this, listaCompraService, x)).ToArray();
            else
                produtosListaCompraViewModel = produtoListaCompra.Select(x => new ProdutoListaCompraViewModel(azureService, this, listaCompraService, x)).ToArray();


            var produtosListaCompra = new ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>>(from e in produtosListaCompraViewModel
                                                                                                                   orderby e.ProdutoListaCompra?.Produto?.Nome
                                                                                                                   group e by e.ProdutoListaCompra?.Produto?.Nome[0] into grupos
                                                                                                                   select new GroupCollection<char, ProdutoListaCompraViewModel>(grupos.Key.GetValueOrDefault(), grupos));
            foreach (var item in produtosListaCompra)
            {
                ProdutosListaCompra.Add(item);
            }
        }

        public ListaDeComprasDetalhePageViewModel(AzureService azureService, ListaCompraService service, UserService userService)
        {
            this.azureService = azureService;
            this.listaCompraService = service;
            PersistListaCompra = new Command(SalvarGrupoUsuarioAsync, PodeCriarGrupoOferta);
            SairListaComprasCommand = new Command(ExecutePermanenciaGrupoAsync, EditandoListaDeCompras);
            SimularComprasCommand = new Command(ExecuteSimularComprasAsync, PodeExcluirLista);
            ListarParticipantesListaCommand = new Command(ExecuteListagemParticipantes);
            ProdutoListaCompraTappedCommand = new Command<ProdutoListaCompraViewModel>(ExecuteProdutoListaTapped);
            ProdutosListaCompra = new ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>>();
            CachedList = new ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>>();
            Members = new ObservableCollection<ParticipanteLista>();
            SearchProduct = new Command<string>(ExecuteSearchProductAsync);
        }

        private void ExecuteProdutoListaTapped(ProdutoListaCompraViewModel produtoListaCompra)
        {
            if (!IsNotSearching)
            {
                if (TransformGroupedCollectionToModel(CachedList).FirstOrDefault(x => x.Produto?.Nome == produtoListaCompra.ProdutoListaCompra.Produto?.Nome &&
                                                                                      x.Produto?.IdTipo == produtoListaCompra.ProdutoListaCompra.Produto?.IdTipo) == null)
                {
                    //está pesquisando
                    produtoListaCompra.IsNotSearching = true;
                    var cached = CachedList.FirstOrDefault(x => x.Key == produtoListaCompra.Produto[0]);
                    if (cached != null)
                        cached.Add(produtoListaCompra);
                    else
                        CachedList.Add(new GroupCollection<char, ProdutoListaCompraViewModel>(produtoListaCompra.Produto[0], new[] { produtoListaCompra }));
                }
                SearchText = string.Empty;
                AtualizarStatus();
            }
        }

        private async void ExecuteSearchProductAsync(string expression)
        {
            if (expression == null)
                return;

            if (expression?.Length == 1 && CachedList.Count == 0)
            {
                IsNotSearching = false;
                OnPropertyChanged(nameof(IsNotSearching));
                CachedList = new ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>>(ProdutosListaCompra);
            }
            ProdutosListaCompra.Clear();

            if (string.IsNullOrEmpty(expression))
            {
                IsNotSearching = true;
                OnPropertyChanged(nameof(IsNotSearching));
                foreach (var item in CachedList)
                {
                    ProdutosListaCompra.Add(item);
                }
                await listaCompraService.LoadAutoCompleteAsync(TransformGroupedCollectionToModel(ProdutosListaCompra).Select(x => x.Produto?.Id).ToArray());
                CachedList.Clear();
                return;
            }

            var produtos = await listaCompraService.ObterProdutoPorNome(expression);
            if (produtos != null)
                GroupCollection(produtos);
        }

        private async void ExecuteListagemParticipantes(object obj)
        {
            await PushAsync<ListaDeComprasPageParticipantesViewModel>(false, null, new Autofac.NamedParameter("listaCompraDetalheViewModel", this));
        }

        private async void ExecutePermanenciaGrupoAsync()
        {
            if (await MessageDisplayer.Instance.ShowAskAsync("Deixar a lista de compras", $"Você tem certeza que deseja sair da lista de compras? Você só poderá retornar ao ser convidado", "Sim", "Não"))
            {
                var membro = Members.First(x => x.IdUser == azureService.CurrentUser.User.Id);
                Members.Remove(membro);
                await listaCompraService.ExcluirParticipanteListaCompraAsync(membro);
                await PopAsync<ListaDeComprasPageViewModel>();
            }
        }

        private void AtualizarStatus()
        {
            PersistListaCompra.ChangeCanExecute();
            SairListaComprasCommand.ChangeCanExecute();
            SimularComprasCommand.ChangeCanExecute();
            OnPropertyChanged(nameof(SearchText));
        }

        private async void ExecuteSimularComprasAsync()
        {
            await PushModalAsync<LoadingPageViewModel>(null, new NamedParameter("operation", Operation.Login));
            var simulacao = await listaCompraService.SimularCompraAsync(TransformEditGrupoOferta());
            //var estabelecimentos = simulacao.Estabelecimentos.Select(x => $"{x.NomeEstabelecimento} - R$ {x.TotalSimulacao}");
            await PopModalAsync();
            await PushAsync<SimulacaoCompraPageViewModel>(false, null, new NamedParameter("simulacaoCompra", simulacao));


            //var EstabelecimentoDetalhe = await MessageDisplayer.Instance.ShowOptionsAsync("Melhor custo benefício", "Cancelar", "Teste", estabelecimentos.ToArray());
            //{
            //    //ir para a tela detalhada fazer a magica.
            //    await PopAsync<ListaDeComprasPageViewModel>();
            //}
        }
        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                EditingList = true;
                EditList = await listaCompraService.CarregarListaCompraPorIdAsync(parameters["ID"]);
                AdapterAsync();
            }
            else
            {
                EditList = new ListaCompra();
                EditingList = false;
                var user = azureService.CurrentUser.User;
                AdicionarParticipante(new ParticipanteLista(user.Id) { User = user, Owner = true });
            }
        }

        internal ParticipanteLista AdicionarParticipante(ParticipanteLista participante)
        {
            if (Members?.FirstOrDefault(x => participante.IdUser == x.IdUser) == null)
            {
                participante.NomeLista = Name;
                Members?.Add(participante);
            }
            PersistListaCompra.ChangeCanExecute();
            return participante;
        }

        private ListaCompra TransformEditGrupoOferta()
        {
            EditList.Nome = Name;
            EditList.Participantes = Members;
            EditList.ProdutosListaCompra = TransformGroupedCollectionToModel(ProdutosListaCompra);
            return EditList;
        }

        private IEnumerable<ProdutoListaCompra> TransformGroupedCollectionToModel(ObservableCollection<GroupCollection<char, ProdutoListaCompraViewModel>> produtosListaCompra)
        {
            return produtosListaCompra.SelectMany(x => x).Select(x => x.Transform()).ToArray();
        }

        private async void AdapterAsync()
        {
            Name = EditList.Nome;
            EditList.Participantes = await listaCompraService.CarregarParticipantesPorIdListaDeCompraAsync(EditList.Id);
            EditList.ProdutosListaCompra = await listaCompraService.CarregarProdutosListaCompraPorIdListaDeCompraAsync(EditList.Id);
            GroupCollection(null, EditList.ProdutosListaCompra);
            await listaCompraService.LoadAutoCompleteAsync(TransformGroupedCollectionToModel(ProdutosListaCompra).Select(x => x.Produto?.Id).ToArray());
            foreach (var participante in EditList.Participantes)
            {
                participante.NomeLista = Name;
                AdicionarParticipante(participante);
            }
            AtualizarStatus();
        }

        public async Task SalvarEdicaoAsync(bool syncronize = false)
        {
            await listaCompraService.AtualizarListaDeComprasAsync(TransformEditGrupoOferta(), syncronize);
        }

        private async void SalvarGrupoUsuarioAsync()
        {
            if (EditingList)
            {
                await SalvarEdicaoAsync();
            }
            else
            {
                var lista = new ListaCompra(Name);
                await listaCompraService.CadastrarNovaListaDeCompraAsync(TransformEditGrupoOferta());
            }
            await PopAsync<ListaDeComprasPageViewModel>();
        }

        private bool UsuarioLogadoCriouALista => Members.FirstOrDefault(x => x.IdUser == azureService.CurrentUser.User.Id)?.Owner ?? false;

        private bool EditandoListaDeCompras() => EditingList;

        private bool PodeExcluirLista() => EditandoListaDeCompras() && UsuarioLogadoCriouALista;

        private bool PodeCriarGrupoOferta()
        {
            if (EditandoListaDeCompras())
                return true;
            else
                return !string.IsNullOrWhiteSpace(Name);
        }
    }
}
