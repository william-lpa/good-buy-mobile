using GoodBuy.Models;
using GoodBuy.Service;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using GoodBuy.Models.Many_to_Many;
using System.Collections.Generic;
using System.Globalization;

namespace GoodBuy.ViewModels
{
    public class OfertaDetalhePageViewModel : BaseViewModel
    {
        private readonly OfertasService ofertasService;
        private readonly AzureService azureService;
        private string produto;
        private string tipo;
        private float quantidade;
        private string unidadeMedida;
        private string marca;
        private string categoria;
        private string estabelecimento;
        private string preco;

        public string Produto
        {
            get => produto;
            set
            {
                SetProperty(ref produto, value);
                AtualizarStatus();
            }
        }
        public string Tipo
        {
            get => tipo;
            set
            {
                SetProperty(ref tipo, value);
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
        public string Preco
        {
            get => preco;
            set
            {
                SetProperty(ref preco, value);
                AtualizarStatus();
            }
        }
        public decimal PrecoDecimal => decimal.Parse(Preco ?? "0");
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
        private bool editingOferta;
        public bool NotEditingOferta => !EditingOferta;
        public bool EditingOferta
        {
            get => editingOferta;
            set
            {
                SetProperty(ref editingOferta, value);
                Init();
            }
        }
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
            PrimaryAction = new Command(ExecutePersistOfertaAsync, VerificarCamposObrigatorios);
        }

        protected async override void Init(Dictionary<string, string> parameters = null)
        {
            if (EditingOferta || (parameters != null && parameters.ContainsKey("ID")))
            {
                while (OfertasTabDetailPageViewModel.Oferta == null) await Task.Delay(55);

                editOferta = OfertasTabDetailPageViewModel.Oferta;
                Adapter();
            }
            else
            {
                Initialize();
            }
            AtualizarStatus();
        }

        private void Adapter()
        {
            Produto = editOferta?.CarteiraProduto?.Produto?.Nome;
            Tipo = editOferta?.CarteiraProduto?.Produto?.Tipo?.Nome ?? "Tipo não informado";
            Quantidade = editOferta?.CarteiraProduto?.Produto?.QuantidadeMensuravel ?? 0;
            Marca = editOferta?.CarteiraProduto?.Marca?.Nome ?? "Sem marca";
            UnidadeMedida = editOferta?.CarteiraProduto?.Produto?.UnidadeMedida?.Nome;
            Categoria = editOferta?.CarteiraProduto?.Produto?.Categoria?.Nome ?? "Categoria não informada";
            Preco = (editOferta?.PrecoAtual ?? 0).ToString();
            Estabelecimento = editOferta?.Estabelecimento?.Nome;//talbez historico
        }

        private async void Initialize()
        {
            await UpdateOfertasAsync();
            await CarregarDadosPreenchidosAsync();
        }

        private async Task CarregarDadosPreenchidosAsync()
        {
            await ofertasService.LoadAutoCompleteAsync();
        }

        private async Task UpdateOfertasAsync()
        {
            await ofertasService.SyncronizeBaseDeOfertasAsync();
        }

        private bool VerificarCamposObrigatorios()
        {
            return !string.IsNullOrEmpty(Produto) && PrecoDecimal > 0 &&
                        !string.IsNullOrEmpty(Estabelecimento) && !string.IsNullOrEmpty(UnidadeMedida)
                        && Quantidade > 0;
        }

        private async void ExecutePersistOfertaAsync()
        {
            try
            {
                if (NotEditingOferta)
                {
                    var oferta = new Oferta()
                    {
                        PrecoAtual = PrecoDecimal,
                        CarteiraProduto = new CarteiraProduto()
                        {
                            Marca = new Marca(Marca),
                            Produto = new Produto()
                            {
                                Nome = Produto,
                                Categoria = new Categoria(Categoria),
                                UnidadeMedida = new UnidadeMedida(UnidadeMedida),
                                Tipo = new Tipo(Tipo),
                                QuantidadeMensuravel = Quantidade,
                            }
                        },
                        Estabelecimento = new Models.Estabelecimento(Estabelecimento),
                    };

                    ofertasService.CriarNovaOfertaAsync(oferta);
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
