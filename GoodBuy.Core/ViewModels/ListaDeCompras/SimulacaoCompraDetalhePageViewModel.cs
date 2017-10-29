using GoodBuy.Models.Logical;
using System.Collections.ObjectModel;

namespace GoodBuy.ViewModels.ListaDeCompras
{
    public class SimulacaoCompraDetalhePageViewModel : BaseViewModel
    {
        public EstabelecimentoDto Estabelecimento { get; set; }
        public ObservableCollection<ProdutoDto> ProdutosCompra { get; set; }

        public SimulacaoCompraDetalhePageViewModel(EstabelecimentoDto estabelecimento)
        {
            Estabelecimento = estabelecimento;
            ProdutosCompra = new ObservableCollection<ProdutoDto>(estabelecimento.ProdutosCompra);
        }
    }
}
