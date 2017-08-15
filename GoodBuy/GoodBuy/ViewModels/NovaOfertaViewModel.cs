using GoodBuy.Models;
using GoodBuy.Service;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices;

namespace GoodBuy.ViewModels
{
    class NovaOfertaViewModel : BaseViewModel
    {
        private string produto;
        private string sabor;
        private string quantidade;
        private string unidadeMedida;
        private string marca;
        private string categoria;
        private string estabelecimento;
        private decimal preco;
        private readonly AzureService azure;
        private readonly EntityService<Produto> produtoTable;
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
        public string Quantidade
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
        public ICommand CadastrarOfertaCommand { get; }

        public NovaOfertaViewModel(AzureService azure)
        {
            this.azure = azure;
            //produtoTable = new EntityService<Produto>(azure.Client, );
            teste = azure.GetTable<Produto>();
            CadastrarOfertaCommand = new Command(ExecuteCadastoProduto);
        }

        private async void ExecuteCadastoProduto()
        {
            try
            {
                string id = "3";
                var produto = new Produto() { Id = id, Nome = Produto };
                await teste.InsertAsync(produto);
                await azure.Client.SyncContext.PushAsync();
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }

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
