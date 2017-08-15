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

namespace GoodBuy.ViewModels
{
    class NovaOfertaViewModel : BaseViewModel
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
        private IMobileServiceSyncTable<Produto> teste;

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
        }

        private bool VerificarCamposObrigatorios()
        {
            return true;
        }

        private async void ExecuteCadastroOferta()
        {
            try
            {
                var idSabor = await CriarSabor();
                var idUnidadeMedida = await CriarUnidadeMedida();
                var idCategoria = await CriarCategoria();
                var idMarca = await CriarMarca();
                var idProduto = await CriarProduto(idSabor, idUnidadeMedida, idCategoria);
                var idCarteiraProduto = await CriarCarteiraProduto(idProduto, idMarca);
                var idEstabelecimento = await CriarEstabelecimento();
                //await CriarOferta();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }

        }
        private async Task<string> CriarMarca()
        {
            var repository = GetEntityService<Marca>(Marca);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Marca).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Marca(Marca));
        }
        private async Task<string> CriarEstabelecimento()
        {
            var repository = GetEntityService<Estabelecimento>(Estabelecimento);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Estabelecimento).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Estabelecimento(Estabelecimento));
        }
        private async Task<string> CriarCategoria()
        {
            var repository = GetEntityService<Categoria>(Categoria);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Categoria).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Categoria(Categoria));
        }
        private async Task<string> CriarUnidadeMedida()
        {
            var repository = GetEntityService<UnidadeMedida>(UnidadeMedida);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == UnidadeMedida).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new UnidadeMedida(UnidadeMedida));
        }
        private async Task<string> CriarSabor()
        {
            var repository = GetEntityService<Sabor>(Sabor);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Sabor).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Sabor(Sabor));
        }
        private async Task<string> CriarProduto(string IdSabor = null, string idUnidadeMedida = null, string idCategoria = null)
        {
            var repository = GetEntityService<Produto>(Produto);
            return
                ((await repository?.SyncTableModel.Where(x => x.Nome == Sabor).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new Produto(Produto, IdSabor, idUnidadeMedida, idCategoria, Quantidade));
        }
        private async Task<string> CriarCarteiraProduto(string idProduto, string idMarca)
        {
            var repository = GetEntityService<CarteiraProduto>(Produto);
            if (Marca == null)
                return null;
            return
                ((await repository?.SyncTableModel.Where(x => x.IdMarca == idMarca && x.IdProduto == idProduto).Select(x => x.Id).ToEnumerableAsync()).FirstOrDefault()) ??
                 await repository?.CreateEntity(new CarteiraProduto(idProduto, idMarca));
        }
        public EntityService<T> GetEntityService<T>(string nomeCampoInserindo) where T : class, IEntity
        {
            if (nomeCampoInserindo == null)
                return null;
            return new EntityService<T>(azure.Client, azure.GetTable<T>());
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
