using GoodBuy.Models;
using GoodBuy.Service;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Linq;
using GoodBuy.Model;
using GoodBuy.Models.Many_to_Many;
using System.Collections.Generic;

namespace GoodBuy.ViewModels
{
    public class NovaOfertaViewModel : BaseViewModel
    {
        private readonly AzureService azure;
        private string produto;
        private string sabor;
        private float quantidade;
        private string unidadeMedida;
        private string marca;
        private string categoria;
        private string estabelecimento;
        private decimal preco;
        public List<string> Produtos { get; set; }

        public string Produto
        {
            get => produto;
            set => SetProperty(ref produto, value);
        }
        public string Sabor
        {
            get => sabor;
            set => SetProperty(ref sabor, value);
        }
        public float Quantidade
        {
            get => quantidade;
            set => SetProperty(ref quantidade, value);
        }
        public string UnidadeMedida
        {
            get => unidadeMedida;
            set => SetProperty(ref unidadeMedida, value);
        }
        public string Marca
        {
            get => marca;
            set => SetProperty(ref marca, value);
        }
        public string Categoria
        {
            get => categoria;
            set => SetProperty(ref categoria, value);
        }
        public decimal Preco
        {
            get => preco;
            set => SetProperty(ref preco, value);
        }
        public string Estabelecimento
        {
            get => estabelecimento;
            set => SetProperty(ref estabelecimento, value);
        }
        public ICommand CadastrarOfertaCommand { get; }

        public NovaOfertaViewModel(AzureService azure)
        {
            this.azure = azure;
            CadastrarOfertaCommand = new Command(ExecuteCadastroOferta, VerificarCamposObrigatorios);
            CarregarProdutos();
            new Action(async () => await UpdateOfertasAsync()).Invoke();
        }

        private async void CarregarProdutos()
        {
            //Produtos = new List<string>() { "Arroz", "Feijão", "Couve" };
            Produtos = new List<string>((await new GenericRepository<Produto>(azure).GetEntities()).Select(x => x.Nome));
        }

        private async Task UpdateOfertasAsync()
        {
            var g = System.Diagnostics.Stopwatch.StartNew();
            var t1 = Task.Run(async () => await (new GenericRepository<Sabor>(azure)).PullUpdates());
            var t2 = Task.Run(async () => await (new GenericRepository<UnidadeMedida>(azure)).PullUpdates());
            var t3 = Task.Run(async () => await (new GenericRepository<Categoria>(azure)).PullUpdates());
            await (new GenericRepository<Produto>(azure)).PullUpdates();
            await (new GenericRepository<Marca>(azure)).PullUpdates();
            var t4 = Task.Run(async () => await (new GenericRepository<Estabelecimento>(azure)).PullUpdates());
            await (new GenericRepository<CarteiraProduto>(azure)).PullUpdates();
            await (new GenericRepository<Oferta>(azure)).PullUpdates();
            await (new GenericRepository<HistoricoOferta>(azure)).PullUpdates();
            Task.WaitAll(new Task[] { t1, t2, t3, t4 });
            g.Stop();
            var teste = g.Elapsed.Seconds;
        }

        private bool VerificarCamposObrigatorios()
        {
            return true;
        }

        private async void ExecuteCadastroOferta()
        {
            try
            {
                var produtosCArteiras = await azure.GetTable<CarteiraProduto>().ToListAsync();
                var estabelecimentos = await azure.GetTable<Estabelecimento>().ToListAsync();
                await CriarOferta(produtosCArteiras.First().Id, estabelecimentos.First().Id);
                //await CriarOferta(idCarteiraProduto, idEstabelecimento);
                await PopModalAsync();
                return;

                var idSabor = await CriarSabor();
                var idUnidadeMedida = await CriarUnidadeMedida();
                var idCategoria = await CriarCategoria();
                var idMarca = await CriarMarca();
                var idProduto = await CriarProduto(idSabor, idUnidadeMedida, idCategoria);
                var idEstabelecimento = await CriarEstabelecimento();
                var idCarteiraProduto = await CriarCarteiraProduto(idProduto, idMarca);

            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }

        private async Task CriarOferta(string idCarteira, string idEstabelecimento)
        {
            var repository = await GetEntityService<Oferta>();

            Preco = 1.99M;

            await repository?.CreateEntity(new Oferta(idEstabelecimento, idCarteira, Preco));

            await repository.SyncDataBase();
            //var repository = await GetEntityService<Oferta>();
            //if (idCarteira == null || Preco < 1)
            //    return;

            //if (Estabelecimento != null)
            //{
            //    var ofertaExistente = (await repository.SyncTableModel
            //                        .Where(x => x.IdCarteiraProduto == idCarteira && x.IdEstabelecimento == idEstabelecimento)
            //                        //.OrderByDescending( x => x.)dataCriacao azure;
            //                        .Select(x => x).ToEnumerableAsync()).FirstOrDefault();
            //    if (ofertaExistente != null)
            //    {
            //        if (ofertaExistente.PrecoAtual == Preco) { } //aplicar um "like"
            //        else
            //        {
            //            var historico = (await GetEntityService<HistoricoOferta>()).CreateEntity(new HistoricoOferta(ofertaExistente));
            //            //ofertaExistente.datacreatedOnAzure = DateTime.Now;
            //            ofertaExistente.PrecoAtual = preco;
            //            await repository?.UpdateEntity(ofertaExistente);
            //        }
            //    }
            //    else
            //        await repository?.CreateEntity(new Oferta(idEstabelecimento, idCarteira, Preco));

            //    await repository.SyncDataBase();

        }

        private async Task<string> CriarMarca()
        {
            var repository = await GetEntityService<Marca>();
            if (Marca == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Marca).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Marca(Marca));
        }
        private async Task<string> CriarEstabelecimento()
        {
            var repository = await GetEntityService<Estabelecimento>();
            if (Estabelecimento == null)
                return null;

            await repository.SyncDataBase();
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Estabelecimento).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Estabelecimento(Estabelecimento));
        }
        private async Task<string> CriarCategoria()
        {
            var repository = await GetEntityService<Categoria>();
            if (Categoria == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Categoria).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Categoria(Categoria));
        }
        private async Task<string> CriarUnidadeMedida()
        {
            var repository = await GetEntityService<UnidadeMedida>();
            if (UnidadeMedida == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == UnidadeMedida).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new UnidadeMedida(UnidadeMedida));
        }
        private async Task<string> CriarSabor()
        {
            var repository = await GetEntityService<Sabor>();
            if (Sabor == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Sabor).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Sabor(Sabor));
        }
        private async Task<string> CriarProduto(string IdSabor = null, string idUnidadeMedida = null, string idCategoria = null)
        {
            var repository = await GetEntityService<Produto>();
            if (Produto == null)
                return null;

            var id = ((await repository?.SyncTableModel.Where(x => x.Nome == Produto).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Produto(Produto, IdSabor, idUnidadeMedida, idCategoria, Quantidade));
            await repository.SyncDataBase();
            return id;

        }
        private async Task<string> CriarCarteiraProduto(string idProduto, string idMarca)
        {
            var repository = await GetEntityService<CarteiraProduto>();
            if (Marca == null || Produto == null)
                return null;

            var id = ((await repository?.SyncTableModel.Where(x => x.IdMarca == idMarca && x.IdProduto == idProduto).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                       await repository?.CreateEntity(new CarteiraProduto(idProduto, idMarca));
            await repository.SyncDataBase();
            return id;
        }
        public async Task<GenericRepository<T>> GetEntityService<T>(bool downloadUpdates = false) where T : class, IEntity, new()
        {
            var repository = new GenericRepository<T>(azure);
            if (downloadUpdates)
                await repository.PullUpdates();
            return repository;
        }

        //private ValidatableObject<string> marca;

        //public ValidatableObject<string> Marca
        //{
        //    get => marca;
        //    set => SetProperty(ref marca, value);
        //}
        //public Command ValidateNomeProdutoCommand { get; }

        private void AddValidations()
        {
            //Marca.AddValidations(new RequiredObjectRule<string>());

            ///Produto.AddValidations(new RequiredObjectRule<string>());
        }

        //internal void ValidatableEntries<T>(string sender) where T : IComparable
        //{
        //    //var x = (GetType().GetRuntimeFields().Where(y => y.Name.ToLower() == sender));
        //    //var x1 = x.First().GetValue(this);
        //    //var x2 = x1 as ValidatableObject<T>;
        //    //var x3 = x2.Validate();
        //}
    }
}
