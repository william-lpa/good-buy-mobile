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
    public class OfertaDetalhePageViewModel : BaseViewModel
    {
        private readonly OfertasService ofertasService;
        private readonly AzureService azureService;
        private string produto;
        private string sabor;
        private float quantidade;
        private string unidadeMedida;
        private string marca;
        private string categoria;
        private string estabelecimento;
        private decimal preco;

        public string Produto
        {
            get => produto;
            set
            {
                SetProperty(ref produto, value);
                AtualizarStatus();
            }
        }
        public string Sabor
        {
            get => sabor;
            set
            {
                SetProperty(ref sabor, value);
                AtualizarStatus();
            }
        }
        public float Quantidade
        {
            get => quantidade;
            set
            {
                SetProperty(ref quantidade, value);
                AtualizarStatus();
            }
        }
        public string UnidadeMedida
        {
            get => unidadeMedida;
            set
            {
                SetProperty(ref unidadeMedida, value);
                AtualizarStatus();
            }
        }
        public string Marca
        {
            get => marca;
            set
            {
                SetProperty(ref marca, value);
                AtualizarStatus();
            }
        }
        public string Categoria
        {
            get => categoria;
            set
            {
                SetProperty(ref categoria, value);
                AtualizarStatus();
            }
        }
        public decimal Preco
        {
            get => preco;
            set
            {
                SetProperty(ref preco, value);
                AtualizarStatus();
            }
        }
        public string Estabelecimento
        {
            get => estabelecimento;
            set
            {
                SetProperty(ref estabelecimento, value);
                AtualizarStatus();
            }
        }
        public Command PrimaryAction { get; }
        public bool NotEditingOferta => !EditingOferta;
        public bool EditingOferta { get; private set; }
        public string PrimaryActionText => EditingOferta ? "Confirmar" : "Cadastrar";
        public bool MonitorarOferta { get; set; }
        public string MonitorarOfertaValor { get; set; }
        private Oferta editOferta;


        private void AtualizarStatus()
        {
            OnPropertyChanged(nameof(PrimaryActionText));
            PrimaryAction.ChangeCanExecute();
        }

        public OfertaDetalhePageViewModel(AzureService azure, OfertasService ofertasService)
        {
            this.ofertasService = ofertasService;
            this.azureService = azure;
            PrimaryAction = new Command(ExecutePersistOferta, VerificarCamposObrigatorios);
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            if (parameters != null && parameters.ContainsKey("ID"))
            {
                EditingOferta = true;
                editOferta = await ofertasService.ObterOfertaCompleta(parameters["ID"]);
                Adapter();
            }
            else
            {
                EditingOferta = false;
                Initialize();
            }
            AtualizarStatus();
        }

        private void Adapter()
        {
            Produto = editOferta?.CarteiraProduto?.Produto?.Nome;
            Sabor = editOferta?.CarteiraProduto?.Produto?.Sabor?.Nome;
            Quantidade = editOferta?.CarteiraProduto?.Produto?.QuantidadeMensuravel ?? 0;
            Marca = editOferta?.CarteiraProduto?.Marca?.Nome;
            UnidadeMedida = editOferta?.CarteiraProduto?.Produto?.UnidadeMedida?.Nome;
            Categoria = editOferta?.CarteiraProduto?.Produto?.Categoria?.Nome;
            Preco = editOferta?.PrecoAtual ?? 0;
            Estabelecimento = editOferta?.Estabelecimento?.Nome;//talbez historico
        }

        private async void Initialize()
        {
            await UpdateOfertasAsync();
            await CarregarDadosPreenchidos();
        }

        private async Task CarregarDadosPreenchidos()
        {
            await ofertasService.LoadAutoCompleteAsync();
        }

        private async Task UpdateOfertasAsync()
        {
            await ofertasService.SyncronizeBaseDeOfertas();
        }

        private bool VerificarCamposObrigatorios()
        {
            var retrr = !string.IsNullOrEmpty(Produto) && !string.IsNullOrEmpty(Marca) && Preco > 0 &&
                   !string.IsNullOrEmpty(Estabelecimento) && !string.IsNullOrEmpty(UnidadeMedida) &&
                   Quantidade > 0;
            return retrr;
        }

        private async void ExecutePersistOferta()
        {
            try
            {
                if (NotEditingOferta)
                {
                    var oferta = new Oferta()
                    {
                        PrecoAtual = Preco,
                        CarteiraProduto = new CarteiraProduto()
                        {
                            Marca = new Marca(Marca),
                            Produto = new Produto()
                            {
                                Nome = Produto,
                                Categoria = new Categoria(Categoria),
                                UnidadeMedida = new UnidadeMedida(UnidadeMedida),
                                Sabor = new Sabor(Sabor),
                                QuantidadeMensuravel = Quantidade,
                            }
                        },
                        Estabelecimento = new Models.Estabelecimento(Estabelecimento),
                    };

                    ofertasService.CriarNovaOferta(oferta);
                    await PopModalAsync();
                }
                else
                {
                    //salvar o monitoramento da oferta.

                }
            }
            catch (Exception err)
            {
                Log.Log.Instance.AddLog(err);
            }
        }
    }
}
