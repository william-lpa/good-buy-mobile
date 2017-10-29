using System.Collections.Generic;
using GoodBuy.Models.Many_to_Many;
using GoodBuy.Service.Interfaces;
using System.Linq;
using GoodBuy.Models;
using System.Threading.Tasks;
using GoodBuy.Models.Logical;

namespace GoodBuy.Service
{
    class FiltroSimulacaoCompraService : IFilterSimulacaoCompra
    {
        private readonly OfertasService ofertaService;
        private IEnumerable<ProdutoListaCompra> initialValue;
        private IEnumerable<Estabelecimento> fornecedores;

        public IEnumerable<ProdutoListaCompra> InitialValue
        {
            get => initialValue;
            set => initialValue = value;
        }
        public IEnumerable<Estabelecimento> Estabelecimento
        {
            get => fornecedores;
            set => fornecedores = value;
        }

        public FiltroSimulacaoCompraService(OfertasService ofertasService, IEnumerable<ProdutoListaCompra> produtoListaCompra)
        {
            this.ofertaService = ofertasService;
            initialValue = produtoListaCompra;
        }
        private async Task<IEnumerable<EstabelecimentoDto>> AdapterEstabelecimentoDto()
        {
            List<EstabelecimentoDto> estabelecimentos = new List<EstabelecimentoDto>();
            foreach (var estabelecimento in fornecedores)
            {
                var estabelecimentoDto = new EstabelecimentoDto(estabelecimento.Nome);
                estabelecimentoDto.ProdutosCompra = await AdapterProdutoDto(estabelecimento.Id);
                estabelecimentos.Add(estabelecimentoDto);
            }

            estabelecimentos.Select(x => x.TotalSimulacao = x.ProdutosCompra.Sum(y => y.Valor)).ToArray();

            estabelecimentos.SelectMany(x => x.ProdutosCompra)
                .GroupBy(x => x.ProdutoDescricao).ToArray().Select(x =>
                {
                    var produtosOrdenados = x.OrderBy(y => y.Valor).ToArray();
                    var menorPreco = produtosOrdenados.FirstOrDefault();
                    var maiorPreco = produtosOrdenados.LastOrDefault();
                    var nomeEstabelecimentoMaior = estabelecimentos.Find(estabelecimento => estabelecimento.ProdutosCompra.ToList()
                                                                    .Exists(produto => ReferenceEquals(produto, maiorPreco))).NomeEstabelecimento;
                    var nomeEstabelecimentoMenor = estabelecimentos.Find(estabelecimento => estabelecimento.ProdutosCompra.ToList()
                                                                     .Exists(produto => ReferenceEquals(produto, menorPreco))).NomeEstabelecimento;

                    estabelecimentos.SelectMany(estabelecimento => estabelecimento.ProdutosCompra)
                                                   .Where(produto => produto.ProdutoDescricao == x.Key)
                                                   .Select(produto =>
                                                   {
                                                       produto.EstabelecimentoMaisBarato = nomeEstabelecimentoMenor;
                                                       produto.EstabelecimentoMaisBaratoValor = menorPreco.Valor;
                                                       produto.EstabelecimentoMaisCaro = nomeEstabelecimentoMaior;
                                                       produto.EstabelecimentoMaisCaroValor = maiorPreco.Valor;
                                                       return produto;
                                                   }).ToArray();
                    return x;
                }).ToArray();


            return estabelecimentos;
        }

        private async Task<IEnumerable<ProdutoDto>> AdapterProdutoDto(string idEstabelecimento)
        {
            return await ofertaService.ObterMelhoresEstabelecimentos(idEstabelecimento, initialValue);
        }

        public async Task<SimulacaoCompraDto> GetFilterResult()
        {
            return new SimulacaoCompraDto()
            {
                Estabelecimentos = (await AdapterEstabelecimentoDto()).OrderBy(x => x.TotalSimulacao),
            };
        }
        public async Task<IFilterSimulacaoCompra> FiltrarPorProdutoTipoAsync()
        {
            fornecedores = await ofertaService.ObterEstabelecimentosDoProduto(initialValue.Where(x => x.IdProduto != null).Select(x => (x.IdProduto, x.Produto.IdTipo)));
            return this;
        }

        public async Task<IFilterSimulacaoCompra> FiltrarPorMarcaAsync()
        {
            var fornecedores = await ofertaService.ObterEstabelecimentosDaMarca(initialValue.Where(x => x.IdMarca != null).Select(x => x.IdMarca), this.fornecedores.Select(x => x.Id));
            if (fornecedores != null)
                this.fornecedores = fornecedores;

            return this;
        }

        public async Task<IFilterSimulacaoCompra> FiltrarPorQuantidadeAsync()
        {
            fornecedores = await ofertaService.ObterEstabelecimentosComQuantidadesEMedidas(initialValue.Select(x => (x.QuantidadeMensuravel, x.IdUnidadeMedida)), fornecedores.Select(x => x.Id));
            return this;
        }
    }
}
