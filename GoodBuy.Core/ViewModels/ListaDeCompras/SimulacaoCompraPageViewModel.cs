using Autofac;
using GoodBuy.Models.Logical;
using GoodBuy.Service;
using GoodBuy.ViewModels.ListaDeCompras;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace GoodBuy.ViewModels
{
    public class SimulacaoCompraPageViewModel : BaseViewModel
    {
        private readonly AzureService azureService;

        public ObservableCollection<EstabelecimentoDto> Estabelecimentos { get; }
        public Command DetailedEstabelecimentoCommand { get; set; }
        public SimulacaoCompraPageViewModel(AzureService azureService, SimulacaoCompraDto simulacaoCompra)
        {
            this.azureService = azureService;
            Estabelecimentos = new ObservableCollection<EstabelecimentoDto>(simulacaoCompra.Estabelecimentos);
            DetailedEstabelecimentoCommand = new Command<EstabelecimentoDto>(ExecuteListarProdutosFornecedor);
            OnPropertyChanged(nameof(Estabelecimentos));
        }

        private async void ExecuteListarProdutosFornecedor(EstabelecimentoDto estabelecimento)
        {
            await PushModalAsync<SimulacaoCompraDetalhePageViewModel>(null, new NamedParameter(nameof(estabelecimento), estabelecimento));
        }
    }
}
