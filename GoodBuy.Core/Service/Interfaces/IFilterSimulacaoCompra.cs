using GoodBuy.Models;
using GoodBuy.Models.Logical;
using GoodBuy.Models.Many_to_Many;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoodBuy.Service.Interfaces
{
    public interface IFilterSimulacaoCompra
    {
        IEnumerable<ProdutoListaCompra> InitialValue { get; set; }

        Task<SimulacaoCompraDto> GetFilterResult();

        Task<IFilterSimulacaoCompra> FiltrarPorProdutoTipoAsync();

        Task<IFilterSimulacaoCompra> FiltrarPorMarcaAsync();

        Task<IFilterSimulacaoCompra> FiltrarPorQuantidadeAsync();
    }
}
